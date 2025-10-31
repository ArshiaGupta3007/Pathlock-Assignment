import React, { useState } from "react";

interface TaskFormProps {
  onAdd: (title: string, dueDate?: string, estimatedHours?: number) => void;
}

const TaskForm: React.FC<TaskFormProps> = ({ onAdd }) => {
  const [title, setTitle] = useState("");
  const [dueDate, setDueDate] = useState("");
  const [estimatedHours, setEstimatedHours] = useState("");

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!title.trim()) return;

    onAdd(title, dueDate, estimatedHours ? Number(estimatedHours) : undefined);
    setTitle("");
    setDueDate("");
    setEstimatedHours("");
  };

  return (
    <form
      onSubmit={handleSubmit}
      className="flex flex-col sm:flex-row gap-3 mb-6"
    >
      <div className="flex-1">
        <label htmlFor="task-title" className="sr-only">
          Task Title
        </label>
        <input
          id="task-title"
          name="title"
          type="text"
          placeholder="Task title"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          autoComplete="off"
          className="w-full border border-gray-300 rounded-lg px-4 py-2"
        />
      </div>

      <div>
        <label htmlFor="task-due-date" className="sr-only">
          Due Date
        </label>
        <input
          id="task-due-date"
          name="dueDate"
          type="date"
          value={dueDate}
          onChange={(e) => setDueDate(e.target.value)}
          className="border border-gray-300 rounded-lg px-3 py-2"
        />
      </div>

      <div>
        <label htmlFor="task-hours" className="sr-only">
          Estimated Hours
        </label>
        <input
          id="task-hours"
          name="estimatedHours"
          type="number"
          min="0"
          step="0.5"
          placeholder="Est. hours"
          value={estimatedHours}
          onChange={(e) => setEstimatedHours(e.target.value)}
          className="border border-gray-300 rounded-lg px-3 py-2 w-28"
        />
      </div>

      <button
        type="submit"
        className="bg-blue-600 hover:bg-blue-700 text-white px-5 py-2 rounded-lg"
      >
        Add
      </button>
    </form>
  );
};

export default TaskForm;
