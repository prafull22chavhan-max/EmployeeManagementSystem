import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { forkJoin, Observable } from 'rxjs';
import { DashboardSummary, Department, Employee, EmployeeOption, Project } from './models';

export interface Workspace {
  dashboard: DashboardSummary;
  employees: Employee[];
  departments: Department[];
  projects: Project[];
  employeeOptions: EmployeeOption[];
}

interface EditorData {
  kind: 'employee' | 'department' | 'project';
  id: number | null;
  name: string;
  email: string;
  salary: number;
  departmentId: number;
  isActive: boolean;
  budget: number;
  startDate: string;
  endDate: string;
  status: string;
  employeeIds: number[];
}

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly base = '/api';

  loadWorkspace(): Observable<Workspace> {
    return forkJoin({
      dashboard: this.http.get<DashboardSummary>(`${this.base}/dashboard`),
      employees: this.http.get<Employee[]>(`${this.base}/employees`),
      departments: this.http.get<Department[]>(`${this.base}/departments`),
      projects: this.http.get<Project[]>(`${this.base}/projects`),
      employeeOptions: this.http.get<EmployeeOption[]>(`${this.base}/project-options`),
    });
  }

  save(editor: EditorData): Observable<unknown> {
    const path = `${this.base}/${editor.kind === 'employee' ? 'employees' : editor.kind === 'department' ? 'departments' : 'projects'}`;
    const body = editor.kind === 'employee'
      ? {
          fullName: editor.name,
          email: editor.email,
          salary: editor.salary,
          departmentId: editor.departmentId,
          isActive: editor.isActive,
        }
      : editor.kind === 'department'
        ? { name: editor.name, depBudget: editor.budget }
        : {
            name: editor.name,
            startDate: editor.startDate,
            endDate: editor.endDate,
            status: ['Planned', 'Active', 'Completed', 'OnHold'].indexOf(editor.status),
            employeeIds: editor.employeeIds,
          };

    return editor.id
      ? this.http.put(`${path}/${editor.id}`, body)
      : this.http.post(path, body);
  }

  remove(kind: EditorData['kind'], id: number): Observable<unknown> {
    const path = kind === 'employee' ? 'employees' : kind === 'department' ? 'departments' : 'projects';
    return this.http.delete(`${this.base}/${path}/${id}`);
  }
}
