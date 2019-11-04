import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InstallationAsignComponent } from './installation-asign.component';

describe('InstallationAsignComponent', () => {
  let component: InstallationAsignComponent;
  let fixture: ComponentFixture<InstallationAsignComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InstallationAsignComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InstallationAsignComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
