import {
  Component,
  OnInit,
  QueryList,
  ViewChild,
  ViewChildren,
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CaseEditComponent } from 'src/app/modules/cases/components';
import { ReplyElementDto, TemplateDto } from 'src/app/common/models';
import {
  AuthService,
  CasesService,
  EFormService,
} from 'src/app/common/services';
import { InstallationsService } from '../../../../services';
import { InstallationModel } from '../../../../models';

@Component({
  selector: 'app-installation-case-page',
  templateUrl: './installation-case-page.component.html',
  styleUrls: ['./installation-case-page.component.scss'],
})
export class InstallationCasePageComponent implements OnInit {
  @ViewChildren(CaseEditComponent) editElements: QueryList<CaseEditComponent>;
  @ViewChild('caseConfirmation', { static: false }) caseConfirmation;
  id: number;
  installationId: number;
  templateId: number;
  currentTemplate: TemplateDto = new TemplateDto();
  currentInstallation: InstallationModel = new InstallationModel();
  replyElement: ReplyElementDto = new ReplyElementDto();
  reverseRoute: string;

  get userClaims() {
    return this.authService.userClaims;
  }

  constructor(
    private activateRoute: ActivatedRoute,
    private casesService: CasesService,
    private eFormService: EFormService,
    private router: Router,
    private authService: AuthService,
    private installationsService: InstallationsService
  ) {
    this.activateRoute.params.subscribe((params) => {
      this.id = +params['id'];
      this.installationId = +params['installationId'];
      this.templateId = +params['templateId'];
    });
  }

  ngOnInit() {
    this.loadTemplateInfo();
    this.loadInstallationInfo();
  }

  loadCase() {
    if (!this.id || this.id === 0) {
      return;
    }
    this.casesService
      .getById(this.id, this.currentTemplate.id)
      .subscribe((operation) => {
        if (operation && operation.success) {
          this.replyElement = operation.model;
        }
      });
  }

  loadTemplateInfo() {
    if (this.templateId) {
      this.eFormService.getSingle(this.templateId).subscribe((operation) => {
        if (operation && operation.success) {
          this.currentTemplate = operation.model;
          this.loadCase();
        }
      });
    }
  }

  loadInstallationInfo() {
    if (this.installationId) {
      this.installationsService
        .getSingle(this.installationId)
        .subscribe((operation) => {
          if (operation && operation.success) {
            this.currentInstallation = operation.model;
            this.loadCase();
          }
        });
    }
  }

  goToSection(location: string): void {
    window.location.hash = location;
    setTimeout(() => {
      document.querySelector(location).parentElement.scrollIntoView();
    });
  }
}
