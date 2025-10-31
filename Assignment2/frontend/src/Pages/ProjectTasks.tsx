import React, { useEffect, useState } from "react";
import { useParams, Link } from "react-router-dom";
import { Task } from "../types/Task";
import TaskForm from "../Components/TaskForm";
import TaskList from "../Components/TaskList";
import api from "../api/api";

export default function ProjectTasks() {
  const { id } = useParams<{ id: string }>();
  const [tasks, setTasks] = useState<Task[]>([]);
  const [projectTitle, setProjectTitle] = useState("Loading...");

  const fetchTasks = async () => {
    try {
      const tasksRes = await api.get(`/projects/${id}/project-tasks`);
      setTasks(tasksRes.data);

      try {
        const projRes = await api.get(`/projects/${id}`);
        const project = projRes.data.project || projRes.data.Project || projRes.data;
        setProjectTitle(project.title || project.name || "Untitled Project");
      } catch {
        setProjectTitle("Unknown Project");
      }
    } catch (err: any) {
      console.error("Error fetching tasks:", err.response?.data || err.message);
      alert("Failed to load tasks. Please check your API connection.");
    }
  };

  useEffect(() => {
    if (id) fetchTasks();
  }, [id]);

  const addTask = async (title: string, dueDate?: string) => {
    try {
      if (!title.trim()) return alert("Please enter a valid task title.");

      const payload = { title, dueDate: dueDate ? new Date(dueDate).toISOString() : null };
      await api.post(`/projects/${id}/tasks`, payload);
      fetchTasks();
    } catch (err: any) {
      console.error("Error adding task:", err.response?.data || err.message);
      alert("Failed to add task.");
    }
  };

  const toggleTask = async (taskId: number) => {
    try {
      const task = tasks.find((t) => t.id === taskId);
      if (!task) return;
      const payload = { ...task, isCompleted: !task.isCompleted };
      await api.put(`/projects/${id}/tasks/${taskId}`, payload);
      fetchTasks();
    } catch (err: any) {
      console.error("Error toggling task:", err.response?.data || err.message);
    }
  };

  const deleteTask = async (taskId: number) => {
    try {
      if (!confirm("Are you sure you want to delete this task?")) return;
      await api.delete(`/projects/${id}/tasks/${taskId}`);
      fetchTasks();
    } catch (err: any) {
      console.error("Error deleting task:", err.response?.data || err.message);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 py-10">
      <div className="max-w-3xl mx-auto bg-white shadow-lg rounded-2xl p-8">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-2xl font-semibold text-blue-700">
            📋 Tasks for {projectTitle}
          </h1>
          <Link to="/dashboard" className="text-sm text-blue-600 hover:underline">
            ← Back to Projects
          </Link>
        </div>

        <TaskForm onAdd={addTask} />
        <TaskList tasks={tasks} onToggle={toggleTask} onDelete={deleteTask} />
      </div>
    </div>
  );
}
