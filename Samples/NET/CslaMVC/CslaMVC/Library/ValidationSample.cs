using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Csla;

namespace CslaMVC.Library
{
    [Serializable]
    public class ValidationSample : BusinessBase<ValidationSample>
    {
        const string ValidateGuid = @"^[a-fA-F\d]{8}-([a-fA-F\d]{4}-){3}[a-fA-F\d]{12}$";
        const string ValidateNonEmptyGuid = @"^(?!0{8})[a-fA-F\d]{8}-((?!0{4})[a-fA-F\d]{4}-){3}(?!0{12})[a-fA-F\d]{12}$";
        const string ValidateSSN = @"^\d{3}-\d{2}-\d{4}$";
        const string ValidateZipCode = @"^\d{5}(-\d{4})?$"; // @"^\d{5}$";
        const string ValidateUrl = @"^http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
        const string ValidateEMail = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        const string ValidateYear = @"^\d{4}$";
        const string ValidateAreaCode = @"^\d{3}$";
        const string ValidatePhoneNumber = @"^\d{3}-\d{4}$";
        const string ValidatePhoneExtension = @"^\d{3}$";

        #region Business Methods

        public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(o => o.Id, "Id");
        [Range(1, int.MaxValue)]
        public int Id
        {
            get { return GetProperty(IdProperty); }
            set { SetProperty(IdProperty, value); }
        }

        public static readonly PropertyInfo<int?> NumProperty = RegisterProperty<int?>(o => o.Num, "Required Int");
        [Required]
        public int? Num
        {
            get { return GetProperty(NumProperty); }
            set { SetProperty(NumProperty, value); }
        }

        public static readonly PropertyInfo<string> SLengthProperty = RegisterProperty<string>(o => o.SLength, "String Max Length");
        [StringLength(10, MinimumLength = 2)]
        public string SLength
        {
            get { return GetProperty(SLengthProperty); }
            set { SetProperty(SLengthProperty, value); }
        }

        public static readonly PropertyInfo<string> SMinLengthProperty = RegisterProperty<string>(o => o.SMinLength, "String Min Length");
        [StringLength(10, MinimumLength = 2)]
        public string SMinLength
        {
            get { return GetProperty(SMinLengthProperty); }
            set { SetProperty(SMinLengthProperty, value); }
        }

        public static readonly PropertyInfo<byte> CodeProperty = RegisterProperty<byte>(o => o.Code, "ByteCode");
        [Range(typeof(byte), "1", "4")]
        public byte Code
        {
            get { return GetProperty(CodeProperty); }
            set { SetProperty(CodeProperty, value); }
        }

        public static readonly PropertyInfo<string> UidProperty = RegisterProperty<string>(o => o.Uid, "Guid");
        [RegularExpression(ValidateNonEmptyGuid)]
        public string Uid
        {
            get { return GetProperty(UidProperty); }
            set { SetProperty(UidProperty, value); }
        }

        public static readonly PropertyInfo<string> SSNProperty = RegisterProperty<string>(o => o.SSN, "SSN");
        [RegularExpression(ValidateSSN)]
        public string SSN
        {
            get { return GetProperty(SSNProperty); }
            set { SetProperty(SSNProperty, value); }
        }

        public static readonly PropertyInfo<string> ZipProperty = RegisterProperty<string>(o => o.Zip, "Zip");
        [RegularExpression(ValidateZipCode)]
        public string Zip
        {
            get { return GetProperty(ZipProperty); }
            set { SetProperty(ZipProperty, value); }
        }

        public static readonly PropertyInfo<string> UrlProperty = RegisterProperty<string>(o => o.Url, "Url");
        [RegularExpression(ValidateUrl)]
        public string Url
        {
            get { return GetProperty(UrlProperty); }
            set { SetProperty(UrlProperty, value); }
        }

        public static readonly PropertyInfo<string> EmailProperty = RegisterProperty<string>(o => o.Email, "E-Mail");
        [RegularExpression(ValidateEMail)]
        public string Email
        {
            get { return GetProperty(EmailProperty); }
            set { SetProperty(EmailProperty, value); }
        }

        public static readonly PropertyInfo<string> YearProperty = RegisterProperty<string>(o => o.Year, "Year");
        [RegularExpression(ValidateYear)]
        public string Year
        {
            get { return GetProperty(YearProperty); }
            set { SetProperty(YearProperty, value); }
        }

        public static readonly PropertyInfo<string> AreaCodeProperty = RegisterProperty<string>(o => o.AreaCode, "AreaCode");
        [RegularExpression(ValidateAreaCode)]
        public string AreaCode
        {
            get { return GetProperty(AreaCodeProperty); }
            set { SetProperty(AreaCodeProperty, value); }
        }

        public static readonly PropertyInfo<string> PhoneProperty = RegisterProperty<string>(o => o.Phone, "Phone");
        [RegularExpression(ValidatePhoneNumber)]
        public string Phone
        {
            get { return GetProperty(PhoneProperty); }
            set { SetProperty(PhoneProperty, value); }
        }

        public static readonly PropertyInfo<string> ExtensionProperty = RegisterProperty<string>(o => o.Extension, "Extension");
        [RegularExpression(ValidatePhoneExtension)]
        public string Extension
        {
            get { return GetProperty(ExtensionProperty); }
            set { SetProperty(ExtensionProperty, value); }
        }
        #endregion

        #region Validation Rules

        protected override void AddBusinessRules()
        {
            //if overridden, this method must be called to included data annotations
            BusinessRules.AddDataAnnotations();

            //TODO: add othe rules
            //ValidationRules.AddRule(RuleMethod, "");
        }

        #endregion

        #region Factory Methods

        public static ValidationSample NewSample()
        {
            return DataPortal.Create<ValidationSample>();
        }

        public static ValidationSample GetSample(int id)
        {
            return DataPortal.Fetch<ValidationSample>(
              new SingleCriteria<ValidationSample, int>(id));
        }

        public static void DeleteSample(int id)
        {
            DataPortal.Delete<Customer>(new SingleCriteria<ValidationSample, int>(id));
        }

      public ValidationSample()
        { /* Require use of factory methods */ }

        #endregion

        #region Data Access

        [RunLocal]
        protected override void DataPortal_Create()
        {
            // TODO: load default values
            // omit this override if you have no defaults to set
            base.DataPortal_Create();
        }

        private void DataPortal_Fetch(SingleCriteria<ValidationSample, int> criteria)
        {
            // TODO: load values
        }

        [Transactional(TransactionalTypes.TransactionScope)]
        protected override void DataPortal_Insert()
        {
            // TODO: insert values
        }

        [Transactional(TransactionalTypes.TransactionScope)]
        protected override void DataPortal_Update()
        {
            // TODO: update values
        }

        [Transactional(TransactionalTypes.TransactionScope)]
        protected override void DataPortal_DeleteSelf()
        {
            DataPortal_Delete(new SingleCriteria<ValidationSample, int>(this.Id));
        }

        [Transactional(TransactionalTypes.TransactionScope)]
        private void DataPortal_Delete(SingleCriteria<ValidationSample, int> criteria)
        {
            // TODO: delete values
        }

        #endregion
    }
}
