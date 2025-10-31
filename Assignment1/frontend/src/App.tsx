import React, { useEffect, useState } from "react";
import { Task } from "./types";

const App: React.FC = () => {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [newTask, setNewTask] = useState("");
  const [dueDate, setDueDate] = useState("");
  const [searchTerm, setSearchTerm] = useState("");
  const [filter, setFilter] = useState<"all" | "completed" | "active">("all");
  const [error, setError] = useState("");

  useEffect(() => {
    const saved = localStorage.getItem("tasks");
    if (saved) setTasks(JSON.parse(saved));
  }, []);

  useEffect(() => {
    localStorage.setItem("tasks", JSON.stringify(tasks));
  }, [tasks]);

  const handleAdd = () => {
    if (!newTask.trim()) {
      setError("Please enter a task description.");
      return;
    }
    if (!dueDate) {
      setError("Please select a due date.");
      return;
    }

    setError("");
    const newTaskItem: Task = {
      id: Date.now(),
      description: newTask.trim(),
      dueDate,
      isCompleted: false,
    };

    setTasks([...tasks, newTaskItem]);
    setNewTask("");
    setDueDate("");
  };

  const toggleComplete = (id: number) => {
    setTasks(tasks.map((t) => (t.id === id ? { ...t, isCompleted: !t.isCompleted } : t)));
  };

  const handleDelete = (id: number) => {
    setTasks(tasks.filter((t) => t.id !== id));
  };
  function formatDate(value?: string | null) {
  if (!value) return "No due date";
  const d = new Date(value);
  if (Number.isNaN(d.getTime())) return "Invalid date";
  return d.toLocaleDateString(); 
}

  const filteredTasks = tasks
    .filter((task) => {
      if (filter === "completed") return task.isCompleted;
      if (filter === "active") return !task.isCompleted;
      return true;
    })
    .filter((task) => task.description.toLowerCase().includes(searchTerm.toLowerCase()));

  return (
    <div className="min-h-screen bg-blue-200 flex flex-col items-center py-32 px-4">
      <div className="w-full mt-100 max-w-2xl bg-white rounded-xl shadow-xl p-8 border border-gray-100">
        <h1 className="text-3xl font-bold text-gray-800 text-center mb-8">
          Task Manager
        </h1>

        <div className="flex flex-col sm:flex-row gap-3 mb-2">
          <input
            type="text"
            placeholder="Enter task..."
            value={newTask}
            onChange={(e) => setNewTask(e.target.value)}
            className="flex-1 border border-gray-300 rounded-lg px-4 py-2 text-gray-700 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none transition"
          />
          <input
            type="date"
            value={dueDate}
            onChange={(e) => setDueDate(e.target.value)}
            className="border border-gray-300 rounded-lg px-3 py-2 text-gray-700 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none transition"
          />
          <button
            onClick={handleAdd}
            className="bg-blue-600 hover:bg-blue-700 text-white px-5 py-2 rounded-lg font-semibold transition"
          >
            Add
          </button>
        </div>

        {error && <p className="text-red-500 text-sm mt-1 mb-4">{error}</p>}

        <div className="flex justify-between items-center mb-5 mt-3">
          <input
            type="text"
            placeholder="Search tasks..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full border border-gray-300 rounded-lg px-4 py-2 text-gray-700 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none transition"
          />
        </div>

        <div className="flex justify-center gap-3 mb-6">
          {["all", "active", "completed"].map((f) => (
            <button
              key={f}
              onClick={() => setFilter(f as any)}
              className={`px-4 py-1.5 rounded-md text-sm font-medium transition-all ${
                filter === f
                  ? "bg-blue-600 text-white shadow-sm"
                  : "bg-gray-100 text-gray-700 hover:bg-gray-200"
              }`}
            >
              {f[0].toUpperCase() + f.slice(1)}
            </button>
          ))}
        </div>

        {filteredTasks.length === 0 ? (
          <div className="text-center text-gray-500 py-6">
            <p className="text-lg">No tasks found</p>
            <p className="text-sm text-gray-400">Try adding a new one above</p>
          </div>
        ) : (
          <ul className="space-y-3">
            {filteredTasks.map((task) => (
              <li
                key={task.id}
                className="flex items-center justify-between bg-gray-50 hover:bg-gray-100 border border-gray-200 rounded-lg px-4 py-3 transition"
              >
                <div className="flex items-center gap-3 flex-1">
                  <input
                    type="checkbox"
                    checked={task.isCompleted}
                    onChange={() => toggleComplete(task.id)}
                    className="w-5 h-5 accent-blue-600 cursor-pointer"
                  />
                  <div>
                    <p
                      className={`text-base font-medium ${
                        task.isCompleted ? "line-through text-gray-400" : "text-gray-800"
                      }`}
                    >
                      {task.description}
                    </p>
                    <p className="text-sm text-gray-500">
Due: {formatDate(task.dueDate)}
                    </p>
                  </div>
                </div>
                <button
                  onClick={() => handleDelete(task.id)}
                  className="text-red-500 hover:text-red-600 transition font-medium text-lg"
                >
                  ✕
                </button>
              </li>
            ))}
          </ul>
        )}
      </div>
    </div>
  );
};

export default App;
