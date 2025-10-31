import { useEffect, useState } from "react";
import { useParams, Link } from "react-router-dom";
import api from "../api/api";

interface Task {
  id: number;
  title: string;
  dueDate: string | null;
  isCompleted: boolean;
  projectId: number;
}

export default function TasksPage() {
  const { id } = useParams();
  const [tasks, setTasks] = useState<Task[]>([]);
  const [title, setTitle] = useState("");

  const fetchTasks = async () => {
    try {
      const res = await api.get(`/projects/${id}/tasks`);
      setTasks(res.data);
    } catch (err) {
      console.error(err);
      alert("Failed to load tasks");
    }
  };

  useEffect(() => {
    fetchTasks();
  }, [id]);

  const addTask = async () => {
    if (!title.trim()) return alert("Enter a task title");
    await api.post(`/projects/${id}/tasks`, { title });
    setTitle("");
    fetchTasks();
  };

  const toggleTask = async (taskId: number) => {
    await api.post(`/projects/${id}/tasks/${taskId}/toggle`);
    fetchTasks();
  };

  const deleteTask = async (taskId: number) => {
    if (!confirm("Delete this task?")) return;
    await api.delete(`/projects/${id}/tasks/${taskId}`);
    fetchTasks();
  };

  return (
    <div className="min-h-screen bg-gray-50 py-10">
      <div className="max-w-3xl mx-auto bg-white p-8 rounded-2xl shadow-lg">
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-2xl font-bold text-blue-700">📋 Tasks</h2>
          <Link to="/dashboard" className="text-sm text-blue-600 hover:underline">
            ← Back to Dashboard
          </Link>
        </div>

        <div className="flex gap-2 mb-6">
          <input
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            className="flex-1 border px-4 py-2 rounded-lg focus:ring-2 focus:ring-blue-400"
            placeholder="Enter task title"
          />
          <button
            onClick={addTask}
            className="bg-blue-600 text-white px-5 py-2 rounded-lg hover:bg-blue-700 transition"
          >
            Add Task
          </button>
        </div>

        {tasks.length === 0 ? (
          <p className="text-center text-gray-400 italic">No tasks yet.</p>
        ) : (
          <ul className="space-y-3">
            {tasks.map((t) => (
              <li
                key={t.id}
                className={`flex justify-between items-center p-3 rounded-lg ${
                  t.isCompleted ? "bg-green-50" : "bg-gray-100"
                }`}
              >
                <div>
                  <p
                    className={`font-medium ${
                      t.isCompleted ? "line-through text-gray-500" : "text-gray-800"
                    }`}
                  >
                    {t.title}
                  </p>
                  {t.dueDate && (
                    <span className="text-sm text-gray-500">
                      Due: {new Date(t.dueDate).toLocaleDateString()}
                    </span>
                  )}
                </div>
                <div className="flex gap-3">
                  <button
                    onClick={() => toggleTask(t.id)}
                    className="text-green-600 hover:text-green-800"
                  >
                    ✅
                  </button>
                  <button
                    onClick={() => deleteTask(t.id)}
                    className="text-red-600 hover:text-red-800"
                  >
                    🗑
                  </button>
                </div>
              </li>
            ))}
          </ul>
        )}
      </div>
    </div>
  );
}
