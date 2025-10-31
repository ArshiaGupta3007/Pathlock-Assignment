import { useEffect, useState } from "react";
import { Link, useLocation } from "react-router-dom";
import api from "../api/api";

interface Project {
  id: number;
  title: string;
  createdAt: string;
}

interface ScheduleResponse {
  recommendedOrder: string[];
}

export default function Dashboard() {
  const [projects, setProjects] = useState<Project[]>([]);
  const [taskCounts, setTaskCounts] = useState<Record<number, number>>({});
  const [title, setTitle] = useState("");
  const [search, setSearch] = useState("");
  const [loading, setLoading] = useState(false);
  const [schedulingId, setSchedulingId] = useState<number | null>(null);
  const [recommendedOrders, setRecommendedOrders] = useState<
    Record<number, string[]>
  >({});
  const location = useLocation();

  const fetchProjects = async () => {
    try {
      setLoading(true);
      const res = await api.get("/projects");
      const fetched = res.data;
      setProjects(fetched);

      const counts: Record<number, number> = {};
      for (const project of fetched) {
        try {
          const tasksRes = await api.get(`/projects/${project.id}/tasks`);
          counts[project.id] = tasksRes.data.length;
        } catch {
          counts[project.id] = 0;
        }
      }
      setTaskCounts(counts);
    } catch (err) {
      console.error(err);
      alert("Failed to fetch projects");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchProjects();
  }, [location.key]);

  const createProject = async () => {
    if (!title.trim()) return alert("Please enter a project title");
    await api.post("/projects", { title });
    setTitle("");
    fetchProjects();
  };

  const deleteProject = async (id: number) => {
    if (!confirm("Are you sure you want to delete this project?")) return;
    await api.delete(`/projects/${id}`);
    fetchProjects();
  };

  const runSmartScheduler = async (id: number) => {
    setSchedulingId(id);
    try {
      const tasksRes = await api.get(`/projects/${id}/tasks`);
      const scheduleRes = await api.post<ScheduleResponse>(
        `/projects/${id}/schedule`
      );

      setRecommendedOrders((prev) => ({
        ...prev,
        [id]: scheduleRes.data.recommendedOrder,
      }));
    } catch (err) {
      console.error("Error scheduling tasks", err);
      alert("Failed to generate schedule. Make sure project has tasks.");
    } finally {
      setSchedulingId(null);
    }
  };

  const filtered = projects.filter((p) =>
    p.title.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <div className="min-h-screen bg-gray-50 py-10">
      <div className="max-w-4xl mx-auto bg-white shadow-lg rounded-2xl p-8">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-3xl font-semibold text-blue-700">
            📂 Project Dashboard
          </h1>
          <button
            onClick={fetchProjects}
            className="text-sm text-blue-600 hover:underline"
          >
            🔄 Refresh
          </button>
        </div>

        <div className="flex gap-2 mb-6">
          <input
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            className="flex-1 border px-4 py-2 rounded-lg focus:ring-2 focus:ring-blue-400"
            placeholder="Enter new project title"
          />
          <button
            onClick={createProject}
            className="bg-blue-600 text-white px-5 py-2 rounded-lg hover:bg-blue-700 transition"
          >
            Add Project
          </button>
        </div>

        <input
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="w-full border px-4 py-2 rounded-lg mb-4 focus:ring-2 focus:ring-blue-400"
          placeholder="🔍 Search projects..."
        />

        {loading ? (
          <p className="text-center text-gray-500">Loading projects...</p>
        ) : filtered.length === 0 ? (
          <p className="text-center text-gray-400 italic">No projects found.</p>
        ) : (
          <ul className="space-y-4">
            {filtered.map((p) => (
              <li
                key={p.id}
                className="bg-gray-100 hover:bg-gray-200 px-4 py-4 rounded-xl transition"
              >
                <div className="flex justify-between items-center">
                  <div className="flex flex-col flex-1">
                    <Link
                      to={`/project/${p.id}/tasks`}
                      className="text-lg font-medium text-blue-700 hover:underline"
                    >
                      {p.title}
                    </Link>
                    <span className="text-sm text-gray-500">
                      🗓 {new Date(p.createdAt).toLocaleDateString()} • 📋{" "}
                      {taskCounts[p.id] ?? 0} tasks
                    </span>
                  </div>

                  <div className="flex gap-2">
                    <button
                      onClick={() => deleteProject(p.id)}
                      className="text-red-600 hover:text-red-800"
                    >
                      🗑 
                    </button>
                    <button
                      onClick={() => runSmartScheduler(p.id)}
                      className="bg-purple-600 text-white px-3 py-1 rounded-lg hover:bg-purple-700"
                      disabled={schedulingId === p.id}
                    >
                      {schedulingId === p.id
                        ? "⏳ Scheduling..."
                        : "⚙️ Smart Schedule"}
                    </button>
                  </div>
                </div>

                {recommendedOrders[p.id] && (
                  <div className="mt-3 bg-white border border-purple-200 rounded-lg p-3">
                    <p className="font-semibold text-purple-700 mb-1">
                      Recommended Order:
                    </p>
                    <ol className="list-decimal ml-5 text-sm text-gray-700">
                      {recommendedOrders[p.id].map((task, idx) => (
                        <li key={idx}>{task}</li>
                      ))}
                    </ol>
                  </div>
                )}
              </li>
            ))}
          </ul>
        )}
      </div>
    </div>
  );
}
