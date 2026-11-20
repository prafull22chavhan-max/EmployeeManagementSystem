import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { App } from './app';
import { ApiService } from './api.service';

describe('App', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [{
        provide: ApiService,
        useValue: {
          loadWorkspace: () => of({
            dashboard: {
              totalEmployees: 0,
              activeEmployees: 0,
              totalDepartments: 0,
              totalProjects: 0,
              activeProjects: 0,
              totalAnnualPayroll: 0,
              departmentDistribution: [],
              projectsByStatus: {},
              recentEmployees: [],
            },
            employees: [],
            departments: [],
            projects: [],
            employeeOptions: [],
          }),
        },
      }],
    })
      .compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should render the workspace dashboard', async () => {
    const fixture = TestBed.createComponent(App);
    await fixture.whenStable();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h1')?.textContent).toContain('team');
  });
});
