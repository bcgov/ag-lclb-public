import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { FormBase } from '@shared/form-base';
import { filter, flatMap } from 'rxjs/operators';
import { ApplicationDataService } from '@services/application-data.service';
import { MatSnackBar } from '@angular/material';
import { DelayedFileUploaderComponent } from '@shared/components/delayed-file-uploader/delayed-file-uploader.component';
import { throwError, Observable, forkJoin } from 'rxjs';
import { FileItem } from '../../../models/file-item.model';
import { FileDataService } from '../../../services/file-data.service';

const FormValidationErrorMap = {
  description1: 'Licence Number',
  licenceType: 'Licence Type',
  nameOfApplicant: 'Licensee name',
  establishmentName: 'Establishment name',
  establishmentAddressStreet: 'Establishmen Address Street',
  establishmentAddressCity: 'Establishment Address City',
  establishmentAddressPostalCode: 'Establishment Address city',
  contactPersonPhone: 'Business Telephone',
  contactPersonEmail: 'Business Email',
  contactPersonFirstName: 'Contact First Name',
  contactPersonLastName: 'Contact Last Name',
  contactPersonRole: 'Contact Title/Position',
  addressStreet: 'Mailing Address Street',
  addressCity: 'Mailing Address City',
  addressPostalCode: 'Mailing Address Postal Code',

  //receivedLGPermission: 'Local Government Permission Receipt checkbox',
  lgStatus: 'Selection of the LG Option',
  signatureAgreement: 'Declaration checkbox',

  currentTotalCapicityIncluded: 'Current Total Capacity checkbox',
  areasToBeExtendedIncluded: 'Areas to be Extended checkbox',
  floorPlanIncluded: 'Floor Plan checkbox',
  writtenApprovalIncluded: 'Written Approval checkbox',
};

@Component({
  selector: 'app-application-covid-temporary-extension',
  templateUrl: './application-covid-temporary-extension.component.html',
  styleUrls: ['./application-covid-temporary-extension.component.scss']
})
export class ApplicationCovidTemporaryExtensionComponent extends FormBase implements OnInit {
  form: FormGroup;
  busy: any;
  showValidationMessages: boolean;
  validationMessages: string[] = [];
  files: FileItem[] = [];
  uploadedFloorplanDocuments: any;
  uploadedLicenseeRepresentativeNotficationFormDocuments: any;
  

  @ViewChild('floorplanDocuments', { static: false }) floorplanDocuments: DelayedFileUploaderComponent;
  @ViewChild('licenseeRepresentativeNotficationFormDocuments', { static: false }) licenseeRepresentativeNotficationFormDocuments: DelayedFileUploaderComponent;

  constructor(private fb: FormBuilder,
    private applicationDataService: ApplicationDataService,
        private fileDataService: FileDataService,
        private snackBar: MatSnackBar) {
    super();
  }

  ngOnInit() {
    this.form = this.fb.group({
      description1: ['', [Validators.required, Validators.maxLength(7)]], //this holds the licenceNumber
      licenceType: ['', [Validators.required]],
      nameOfApplicant: ['', [Validators.required]],
      establishmentName: ['', [Validators.required]],
      establishmentAddressStreet: ['', [Validators.required]],
      establishmentAddressCity: ['', [Validators.required]],
      establishmentAddressPostalCode: ['', [Validators.required, this.customZipCodeValidator('addressCountry')]],
      contactPersonPhone: ['', [Validators.required]],
      contactPersonEmail: ['', [Validators.required, Validators.email]],
      contactPersonFirstName: ['', [Validators.required]],
      contactPersonLastName: ['', [Validators.required]],
      contactPersonRole: ['', [Validators.required]],
      sameAddresses: [''],
      addressStreet: ['', [Validators.required]],
      addressCity: ['', [Validators.required]],
      addressPostalCode: ['', [Validators.required, this.customZipCodeValidator('addressCountry')]],
      addressCountry: ['Canada'], // only used client side for validation

      //receivedLGPermission: ['', [this.customRequiredCheckboxValidator()]],
      lgStatus: ['', []],
      signatureAgreement: ['', [this.customRequiredCheckboxValidator()]]
    });

    this.form.get('sameAddresses').valueChanges
      .pipe(filter(value => !!value)) // only when the box is checked
      .subscribe(() => {
        // copy the address values over
        this.form.get('addressStreet').setValue(this.form.get('establishmentAddressStreet').value);
        this.form.get('addressCity').setValue(this.form.get('establishmentAddressCity').value);
        this.form.get('addressPostalCode').setValue(this.form.get('establishmentAddressPostalCode').value);
      });

  }



  uploadDocuments(id: string, fileUploader: DelayedFileUploaderComponent): Observable<any> {
    return forkJoin(
      fileUploader.files.map(f => this.fileDataService.uploadPublicCovidDocument(id, fileUploader.documentType, f.file))
    );
  }

  
  lgInputRequired(): boolean {
    
    // if the chose food primary, they don't even see the LG option and it's not required
    if(this.form.get("licenceType").value && this.form.get("licenceType").value == "Food Primary") {
      this.form.get('lgStatus').clearValidators();
      return false;
    }

    // otherwise lgStatus is required
    this.form.get("lgStatus").setValidators([Validators.required]);
    return true;

  }

  lgConfirmationRequired(): boolean {
  
    return this.form.get("lgStatus").value && this.form.get("lgStatus").value == "option2" && this.lgInputRequired();
    
  }

  submitApplication() {
    
    if (!this.form.valid) {
      this.showValidationMessages = true;
      this.markConstrolsAsTouched(this.form);
      this.validationMessages = this.listControlsWithErrors(this.form, FormValidationErrorMap)
    }
    else {
      this.validationMessages = [];
      this.busy = this.applicationDataService.createCovidApplication(this.form.value).pipe()
        .subscribe(result => {
          if (result.id) {
            // now upload the documents.
            forkJoin(this.uploadDocuments(result.id, this.floorplanDocuments),
              this.uploadDocuments(result.id, this.licenseeRepresentativeNotficationFormDocuments)
            ).pipe()
              .subscribe(() => {
                this.snackBar.open('Application Submitted', 'Success', { duration: 2500, panelClass: ['green-snackbar'] });
              },
                err => {
                  this.snackBar.open('Failed to submit application', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
                });
          } else {
            return throwError('Server Error - no ID was returned');
          }
        });
 
    }

  }

}
