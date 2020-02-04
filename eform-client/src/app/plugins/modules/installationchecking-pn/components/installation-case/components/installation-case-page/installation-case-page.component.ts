import {Component, OnInit, QueryList, ViewChild, ViewChildren} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {CasesService} from 'src/app/common/services/cases';
import {EFormService} from 'src/app/common/services/eform';
import {CaseEditElementComponent} from '../../../../../../../modules/cases/components';
import {TemplateDto} from '../../../../../../../common/models/dto';
import {ReplyElementDto} from '../../../../../../../common/models/cases';
import {AuthService} from '../../../../../../../common/services/auth';
import {InstallationsService} from '../../../../services';
import {InstallationModel} from '../../../../models';

@Component({
  selector: 'app-installation-case-page',
  templateUrl: './installation-case-page.component.html',
  styleUrls: ['./installation-case-page.component.scss']
})
export class InstallationCasePageComponent implements OnInit {
  @ViewChildren(CaseEditElementComponent) editElements: QueryList<CaseEditElementComponent>;
  @ViewChild('caseConfirmation') caseConfirmation;
  id: number;
  installationId: number;
  templateId: number;
  currentTemplate: TemplateDto = new TemplateDto;
  currentInstallation: InstallationModel = new InstallationModel();
  replyElement: ReplyElementDto = new ReplyElementDto();

  spinnerStatus = false;
  reverseRoute: string;

  get userClaims() {
    return this.authService.userClaims;
  }

  constructor(private activateRoute: ActivatedRoute,
              private casesService: CasesService,
              private eFormService: EFormService,
              private router: Router,
              private authService: AuthService,
              private installationsService: InstallationsService) {
    this.activateRoute.params.subscribe(params => {
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
    this.casesService.getById(this.id, this.currentTemplate.id).subscribe(operation => {
      if (operation && operation.success) {
        this.replyElement = operation.model;
      }
      this.spinnerStatus = false;
    });
  }

  loadTemplateInfo() {
    if (this.templateId) {
      this.spinnerStatus = true;
      this.eFormService.getSingle(this.templateId).subscribe(operation => {
        if (operation && operation.success) {
          this.currentTemplate = operation.model;
          this.loadCase();
        }
      });
    }
  }

  loadInstallationInfo() {
    if (this.installationId) {
      this.spinnerStatus = true;
      this.installationsService.getSingle(this.installationId).subscribe(operation => {
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
