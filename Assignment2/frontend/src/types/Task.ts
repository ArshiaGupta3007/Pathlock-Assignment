export interface Task {
  id: number;
  title: string;
  dueDate?: string; 
  isCompleted: boolean;
  projectId: number;
  priority?: "High" | "Medium" | "Low";
  estimatedHours?: number;
}
