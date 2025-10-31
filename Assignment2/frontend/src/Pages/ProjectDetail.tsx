import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import api from "../api/api";

export default function ProjectDetail() {
  const { id } = useParams<{ id: string }>();
  const [tasks, setTasks] = useState<any[]>([]);
  const [title, setTitle] = useState("");

  const fetchTasks = async () => {
    const res = await api.get(`/projects/${id}/tasks`);
    setTasks(res.data);
  };

  const addTask = async () => {
    if (!title.trim()) return alert("Enter task title");
    await api.post(`/projects/${id}/tasks`, { title });
    setTitle("");
    fetchTasks();
  };

  useEffect(() => {
    fetchTasks();
  }, []);

  return (
    <div className="min-h-screen bg-gray-50 py-10 px-6">
      <h2 className="text-2xl font-bold mb-4 text-blue-700">📝 Tasks</h2>
      <div className="flex gap-2 mb-4">
        <input
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          placeholder="New task"
          className="flex-1 border px-4 py-2 rounded-lg"
        />
        <button
          onClick={addTask}
          className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700"
        >
          Add
        </button>
      </div>

      <ul className="space-y-2">
        {tasks.map((t) => (
          <li key={t.id} className="bg-white p-3 rounded-lg shadow">
            {t.title}
          </li>
        ))}
      </ul>
    </div>
  );
}
