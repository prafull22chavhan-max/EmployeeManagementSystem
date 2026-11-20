import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from './api.service';
import {
  DashboardSummary,
  Department,
  Employee,
  EmployeeOption,
  Project,
} from './models';

type View = 'dashboard' | 'employees' | 'departments' | 'projects';
type EditorKind = 'employee' | 'department' | 'project';
type Editor = {
  kind: EditorKind;
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
};

const emptyEditor = (kind: EditorKind): Editor => ({
  kind,
  id: null,
  name: '',
  email: '',
  salary: 0,
  departmentId: 0,
  isActive: true,
  budget: 0,
  startDate: new Date().toISOString().slice(0, 10),
  endDate: '',
  status: 'Active',
  employeeIds: [],
});

@Component({
  imports: [CommonModule, FormsModule],
  selector: 'app-root',
  styleUrl: './app.scss',
  templateUrl: './app.html',
})
export class App implements OnInit {
  private readonly api = inject(ApiService);

  readonly view = signal<View>('dashboard');
  readonly dashboard = signal<DashboardSummary | null>(null);
  readonly employees = signal<Employee[]>([]);
  readonly departments = signal<Department[]>([]);
  readonly projects = signal<Project[]>([]);
  readonly employeeOptions = signal<EmployeeOption[]>([]);
  readonly loading = signal(true);
  readonly error = signal('');
  readonly search = signal('');
  readonly editor = signal<Editor | null>(null);
  readonly saving = signal(false);
  readonly toast = signal('');
  readonly confirmDelete = signal<{ kind: EditorKind; id: number; name: string } | null>(null);

  get today(): Date {
    return new Date();
  }

  get greeting(): string {
    const hour = new Date().getHours();
    return hour < 12 ? 'morning' : hour < 18 ? 'afternoon' : 'evening';
  }

  ngOnInit(): void {
    this.loadAll();
  }

  get pageTitle(): string {
    return {
      dashboard: 'Overview',
      employees: 'People',
      departments: 'Departments',
      projects: 'Projects',
    }[this.view()];
  }

  get filteredEmployees(): Employee[] {
    const term = this.search().trim().toLowerCase();
    if (!term) return this.employees();
    return this.employees().filter((employee) =>
      `${employee.fullName} ${employee.email} ${employee.departmentName}`.toLowerCase().includes(term),
    );
  }

  changeView(view: View): void {
    this.view.set(view);
    this.search.set('');
  }

  loadAll(): void {
    this.loading.set(true);
    this.error.set('');
    this.api.loadWorkspace().subscribe({
      next: (workspace) => {
        this.dashboard.set(workspace.dashboard);
        this.employees.set(workspace.employees);
        this.departments.set(workspace.departments);
        this.projects.set(workspace.projects);
        this.employeeOptions.set(workspace.employeeOptions);
        this.loading.set(false);
      },
      error: (error: HttpErrorResponse) => {
        this.error.set(this.errorMessage(error));
        this.loading.set(false);
      },
    });
  }

  openCreate(kind: EditorKind): void {
    const editor = emptyEditor(kind);
    if (kind === 'employee') editor.departmentId = this.departments()[0]?.id ?? 0;
    if (kind === 'project') editor.endDate = editor.startDate;
    this.editor.set(editor);
  }

  openEmployee(employee: Employee): void {
    this.editor.set({
      ...emptyEditor('employee'),
      id: employee.id,
      name: employee.fullName,
      email: employee.email,
      salary: employee.salary,
      departmentId: employee.departmentId,
      isActive: employee.isActive,
    });
  }

  openDepartment(department: Department): void {
    this.editor.set({
      ...emptyEditor('department'),
      id: department.id,
      name: department.name,
      budget: department.depBudget,
    });
  }

  openProject(project: Project): void {
    this.editor.set({
      ...emptyEditor('project'),
      id: project.id,
      name: project.name,
      startDate: project.startDate.slice(0, 10),
      endDate: project.endDate.slice(0, 10),
      status: project.status,
      employeeIds: [...project.employeeIds],
    });
  }

  save(): void {
    const editor = this.editor();
    if (!editor || this.saving()) return;

    this.saving.set(true);
    this.error.set('');
    this.api.save(editor).subscribe({
      next: () => {
        this.editor.set(null);
        this.saving.set(false);
        this.toast.set(`${this.label(editor.kind)} ${editor.id ? 'updated' : 'created'} successfully.`);
        this.loadAll();
      },
      error: (error: HttpErrorResponse) => {
        this.error.set(this.errorMessage(error));
        this.saving.set(false);
      },
    });
  }

  remove(): void {
    const target = this.confirmDelete();
    if (!target) return;
    this.api.remove(target.kind, target.id).subscribe({
      next: () => {
        this.confirmDelete.set(null);
        this.toast.set(`${this.label(target.kind)} removed.`);
        this.loadAll();
      },
      error: (error: HttpErrorResponse) => {
        this.error.set(this.errorMessage(error));
        this.confirmDelete.set(null);
      },
    });
  }

  askDelete(kind: EditorKind, id: number, name: string): void {
    this.error.set('');
    this.confirmDelete.set({ kind, id, name });
  }

  departmentShare(count: number): number {
    const total = this.dashboard()?.totalEmployees ?? 0;
    return total ? Math.max(6, Math.round((count / total) * 100)) : 0;
  }

  initials(name: string): string {
    return name.split(/\s+/).slice(0, 2).map((part) => part[0] ?? '').join('').toUpperCase();
  }

  avatarClass(id: number): string {
    return `avatar-tone-${(id % 5) + 1}`;
  }

  departmentClass(index: number): string {
    return `department-tone-${(index % 5) + 1}`;
  }

  projectClass(index: number): string {
    return `project-tone-${(index % 5) + 1}`;
  }

  statusClass(status: string): string {
    return `status-${status.toLowerCase()}`;
  }

  statusLabel(status: string): string {
    return status === 'OnHold' ? 'On hold' : status;
  }

  label(kind: EditorKind): string {
    return kind === 'employee' ? 'teammate' : kind === 'department' ? 'department' : 'project';
  }

  dismissToast(): void {
    this.toast.set('');
  }

  private errorMessage(error: HttpErrorResponse): string {
    if (typeof error.error === 'string') return error.error;
    if (error.error?.message) return error.error.message;
    const validationErrors = error.error?.errors;
    if (validationErrors && typeof validationErrors === 'object') {
      const messages = Object.values(validationErrors as Record<string, unknown>)
        .flatMap((value) => Array.isArray(value) ? value.map(String) : []);
      if (messages.length) return messages.join(' ');
    }
    if (error.error?.title) return error.error.title;
    return error.status === 0
      ? 'The server could not be reached. Check the connection and try again.'
      : `Something went wrong (${error.status}). Please try again.`;
  }
}
