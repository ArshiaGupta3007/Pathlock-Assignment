import React from "react";
import { Task } from "../types/Task";

interface TaskListProps {
  tasks: Task[];
  onToggle: (id: number) => void;
  onDelete: (id: number) => void;
}

const TaskList: React.FC<TaskListProps> = ({ tasks, onToggle, onDelete }) => {
  if (tasks.length === 0)
    return (
      <p className="text-center text-gray-500 mt-6">
        No tasks found. Add one above!
      </p>
    );

  const formatDate = (d?: string) =>
    d ? new Date(d).toLocaleDateString() : "No due date";

  return (
    <ul className="space-y-3">
      {tasks.map((t) => (
        <li
          key={t.id}
          className="flex justify-between items-center border rounded-lg p-3 bg-gray-50 hover:bg-gray-100 transition"
        >
          <div className="flex items-center gap-3">
            <input
              type="checkbox"
              checked={t.isCompleted}
              onChange={() => onToggle(t.id)}
              className="w-5 h-5 accent-blue-600 cursor-pointer"
            />
            <div>
              <p
                className={`font-medium ${
                  t.isCompleted ? "line-through text-gray-400" : "text-gray-800"
                }`}
              >
                {t.title}
              </p>
              <p className="text-sm text-gray-500">Due: {formatDate(t.dueDate)}</p>
            </div>
          </div>
          <button
            onClick={() => onDelete(t.id)}
            className="text-red-500 hover:text-red-600 font-medium text-lg"
          >
            ✕
          </button>
        </li>
      ))}
    </ul>
  );
};

export default TaskList;
