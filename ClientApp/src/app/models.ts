export interface Employee {
  id: number;
  fullName: string;
  email: string;
  salary: number;
  departmentId: number;
  departmentName: string;
  isActive: boolean;
  projects: string[];
}

export interface Department {
  id: number;
  name: string;
  depBudget: number;
  employeeCount: number;
}

export interface Project {
  id: number;
  name: string;
  startDate: string;
  endDate: string;
  status: string;
  employeeCount: number;
  employeeIds: number[];
}

export interface EmployeeOption {
  id: number;
  fullName: string;
}

export interface DashboardSummary {
  totalEmployees: number;
  activeEmployees: number;
  totalDepartments: number;
  totalProjects: number;
  activeProjects: number;
  totalAnnualPayroll: number;
  departmentDistribution: { id: number; name: string; employeeCount: number }[];
  projectsByStatus: Record<string, number>;
  recentEmployees: Employee[];
}
