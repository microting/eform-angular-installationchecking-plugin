import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InstallationNewComponent } from './installation-new.component';

describe('InstallationNewComponent', () => {
  let component: InstallationNewComponent;
  let fixture: ComponentFixture<InstallationNewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InstallationNewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InstallationNewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
