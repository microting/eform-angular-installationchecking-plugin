import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RemovalPageComponent } from './removal-page.component';

describe('RemovalPageComponent', () => {
  let component: RemovalPageComponent;
  let fixture: ComponentFixture<RemovalPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RemovalPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RemovalPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
