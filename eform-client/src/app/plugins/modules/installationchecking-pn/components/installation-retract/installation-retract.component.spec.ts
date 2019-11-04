import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InstallationRetractComponent } from './installation-retract.component';

describe('InstallationRetractComponent', () => {
  let component: InstallationRetractComponent;
  let fixture: ComponentFixture<InstallationRetractComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InstallationRetractComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InstallationRetractComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
