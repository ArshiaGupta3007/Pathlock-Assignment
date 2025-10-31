const API_URL = "http://localhost:5206/api/tasks";
export const getTasks = async () => {
  const res = await fetch(API_URL);
  if (!res.ok) throw new Error("Failed to load tasks");
  return res.json();
};

export const addTask = async (task: {
  description: string;
  dueDate: string;
}) => {
  const res = await fetch(API_URL, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(task),
  });

  if (!res.ok) throw new Error("Failed to add task");
  return res.json();
};

export const updateTask = async (task: {
  id: number;
  description: string;
  dueDate: string;
  isCompleted: boolean;
}) => {
  const res = await fetch(`${API_URL}/${task.id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(task),
  });

  // Backend may return 204 (no content)
  if (res.status === 204) return task;
  if (!res.ok) throw new Error("Failed to update task");
  return res.json();
};

export const deleteTask = async (id: number) => {
  const res = await fetch(`${API_URL}/${id}`, { method: "DELETE" });
  if (!res.ok) throw new Error("Failed to delete task");
};
