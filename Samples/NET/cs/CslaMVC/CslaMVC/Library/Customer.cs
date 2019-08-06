using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Csla;
using Csla.Data;
using CslaMVC.Repository;
using CslaMVC.Rules;
using Csla.Rules;
using System.Collections.Generic;

namespace CslaMVC.Library
{
    [Serializable]
    public partial class Customer : BusinessBase<Customer>
    {
        #region Business Methods

        public static readonly PropertyInfo<int> CustomerNoProperty = RegisterProperty<int>(o => o.CustomerNo, "Customer No");
        public int CustomerNo
        {
            get { return GetProperty(CustomerNoProperty); }
            internal set { SetProperty(CustomerNoProperty, value); }
        }

        public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(o => o.Name, "Name");
        [Required(ErrorMessage = "'Name' is required")]
        [StringLength(25, ErrorMessage = "Name must be less than 25 characters")]
        public string Name
        {
            get { return GetProperty(NameProperty); }
            set { SetProperty(NameProperty, value); }
        }

        public static readonly PropertyInfo<int> GroupNoProperty = RegisterProperty<int>(o => o.GroupNo, "Group No");
        [Range(0, 9, ErrorMessage = "'GroupNo' must be between 0 and 9")]
        public int GroupNo
        {
            get { return GetProperty(GroupNoProperty); }
            set { SetProperty(GroupNoProperty, value); }
        }

        public static readonly PropertyInfo<string> CityProperty = RegisterProperty<string>(o => o.City, "City");
        public string City
        {
            get { return GetProperty(CityProperty); }
            set { SetProperty(CityProperty, value); }
        }

        //{0} = prop name
        //{1} = rule arg[1]
        public static readonly PropertyInfo<string> StateProperty = RegisterProperty<string>(o => o.State, "State");
        [RegularExpression(@"[a-zA-Z]{2}", ErrorMessage = "'State' must be standard 2 character state code")]
        public string State
        {
            get { return GetProperty(StateProperty); }
            set { SetProperty(StateProperty, value); }
        }

        public static readonly PropertyInfo<string> ZipcodeProperty = RegisterProperty<string>(o => o.Zipcode, "Zipcode");
        [RegularExpression(@"\d{5}(-\d{4})?", ErrorMessage = "'Zipcode' is invalid")]
        public string Zipcode
        {
            get { return GetProperty(ZipcodeProperty); }
            set { SetProperty(ZipcodeProperty, value); }
        }

        public static readonly PropertyInfo<DateTime> StartProperty = RegisterProperty<DateTime>(o => o.Start, "Start");
        public DateTime Start
        {
            get { return GetProperty(StartProperty); }
            set { SetProperty(StartProperty, value); }
        }

        public static readonly PropertyInfo<DateTime> EndProperty = RegisterProperty<DateTime>(o => o.End, "End");
        public DateTime End
        {
            get { return GetProperty(EndProperty); }
            set { SetProperty(EndProperty, value); }
        }
        #endregion

        #region Validation Rules

        protected override void AddBusinessRules()
        {
            base.AddBusinessRules(); //include any data annotation attribute rules

            BusinessRules.AddRule(new Rules.UpperCaseRule(StateProperty) { Priority = 1 }); //modifies data, priority 1 (default is 0)
            BusinessRules.AddRule(new Rules.AlsoRequiredRule(CityProperty, CityProperty, StateProperty)); //multiple related properties
            BusinessRules.AddRule(new Rules.AlsoRequiredRule(StateProperty, CityProperty, StateProperty)); //multiple related properties
            BusinessRules.AddRule(new Rules.IsInListRule<string>(StateProperty, new List<string> { "IL", "MN", "WA" }));

            //BusinessRules.AddRule(new Csla.Rules.CommonRules.Lambda(c => TestRuleAction(c))); //object rule, always broken
            BusinessRules.AddRule(new PrivateRule() { }); //object rule
        }

        protected static void AddObjectAuthorizationRules()
        {
            //auth rules
            Csla.Rules.BusinessRules.AddRule(typeof(Customer), new Csla.Rules.CommonRules.IsInRole(AuthorizationActions.EditObject, new List<string> { "Foo" }));
            Csla.Rules.BusinessRules.AddRule(typeof(Customer), new Csla.Rules.CommonRules.IsInRole(AuthorizationActions.ReadProperty, ZipcodeProperty, "Foo"));
            Csla.Rules.BusinessRules.AddRule(typeof(Customer), new AuthRule(AuthorizationActions.DeleteObject));
        }

        private void TestRuleAction(RuleContext context)
        {
            context.AddErrorResult("lambda rule broken");
        }

        class PrivateRule : BusinessRule
        {
            protected override void Execute(RuleContext context)
            {
                var cust = (Customer)context.Target;
                if (cust.Zipcode == "12345")
                    context.AddErrorResult("private rule failed");
            }
        }

        class AuthRule : AuthorizationRule
        {
            public AuthRule(AuthorizationActions action) : base(action)
            {
            }

            protected override void Execute(AuthorizationContext context)
            {
                //crude custom rule
                context.HasPermission = (DateTime.Now.Minute % 2 == 0);
            }
        }

        #endregion

        #region Factory Methods

        public static Customer NewCustomer()
        {
            return DataPortal.Create<Customer>();
        }

        public static Customer GetCustomer(int CustomerNo)
        {
            return DataPortal.Fetch<Customer>(
              new SingleCriteria<Customer, int>(CustomerNo));
        }

        internal static Customer GetCustomer(object data)
        {
            var customer = new Customer();
            DataMapper.Map(data, customer);
            customer.MarkAsChild();
            return customer;
        }

        public static void DeleteCustomer(int CustomerNo)
        {
            DataPortal.Delete<Customer>(new SingleCriteria<Customer, int>(CustomerNo));
        }

      public Customer()
        { /* Require use of factory methods */ }

        #endregion

        #region Data Access

        public new Customer Save()
        {
            return base.Save();
        }

        [RunLocal]
        protected override void DataPortal_Create()
        {
            var now = DateTime.Now;
            this.Start = now;
            this.End = now;
            base.DataPortal_Create();
        }

        private void DataPortal_Fetch(SingleCriteria<Customer, int> criteria)
        {
            var data = Data.Connect();
            var customer = data.Customers.Where(c => c.CustomerNo == criteria.Value).SingleOrDefault();
            if (customer == null) return;
            BusinessRules.SuppressRuleChecking = true;
            DataMapper.Map(customer, this);
            BusinessRules.SuppressRuleChecking = false;
        }

        protected override void DataPortal_Insert()
        {
            var data = Data.Connect();
            var customer = new CslaMVC.Repository.Customer();
            this.CustomerNo = data.Customers.Select(c => c.CustomerNo).Max() + 1;
            DataMapper.Map(this, customer, "Start", "End");
            data.Customers.Add(customer);
        }

        protected override void DataPortal_Update()
        {
            var data = Data.Connect();
            var customer = data.Customers.Where(c => c.CustomerNo == this.CustomerNo).SingleOrDefault();
            var index = data.Customers.IndexOf(customer);
            if (customer == null) return;
            DataMapper.Map(this, customer, "Start", "End");
            data.Customers[index] = customer;
        }

        protected override void DataPortal_DeleteSelf()
        {
            DataPortal_Delete(new SingleCriteria<Customer, int>(this.CustomerNo));
        }

        private void DataPortal_Delete(SingleCriteria<Customer, int> criteria)
        {
            var data = Data.Connect();
            var customer = data.Customers.Where(c => c.CustomerNo == this.CustomerNo).SingleOrDefault();
            if (customer == null) return;
            data.Customers.Remove(customer);
        }

        #endregion

        #region ICheckRules Members

        /// <summary>
        /// Invokes all rules for the business type.
        /// </summary>
        public void CheckRules()
        {
            BusinessRules.CheckRules();
        }

        /// <summary>
        /// Invokes all business rules attached at the class level of a business type.
        /// </summary>
        public void CheckObjectRules()
        {
            BusinessRules.CheckObjectRules();
        }

        #endregion
    }
}
