using Csla;
using GraphMergerTest.Dal;

namespace GraphMergerTest.Business
{
    [Serializable]
    public class Widget
        : BusinessBase<Widget>
    {
        #region ChildItems

        public static readonly PropertyInfo<Guid> IdProperty = RegisterProperty<Guid>(nameof(Id));
        public Guid Id
        {
            get => GetProperty(IdProperty);
            private set => SetProperty(IdProperty, value);
        }

        public static readonly PropertyInfo<ChildItems> ChildItemsProperty
                         = RegisterProperty<ChildItems>(nameof(ChildItems),
                                                        "ChildItems",
                                                        null,
                                                        RelationshipTypes.LazyLoad);
        public ChildItems ChildItems
        {
            get => LazyGetProperty(ChildItemsProperty, NewChildItems);
            private set => SetProperty(ChildItemsProperty, value);
        }

        private ChildItems NewChildItems()
        {
            var dataPortal = ApplicationContext.GetRequiredService<IDataPortal<ChildItems.Factory>>();

            return ChildItems.New(dataPortal);
        }

        #endregion

        #region Factory

        public class Factory
            : FactoryBase<Widget>
        {
            public Widget NewWidget()
            {
                return DataPortal.Create();
            }

            public async Task<Widget> NewWidgetAsync()
            {
                return await DataPortal.CreateAsync();
            }

            public Factory(IDataPortal<Widget> dataPortal)
                : base(dataPortal)
            {
            }
        }

        #endregion

        #region Data access

        [Create]
        private void Create()
        {
            using (BypassPropertyChecks)
                Id = Guid.NewGuid();
        }

        [Fetch]
        private async Task FetchAsync(Guid                         id,
                             [Inject] IWidgetDal                   dal,
                             [Inject] IChildDataPortal<ChildItems> portalChildItems)
        {
            var dto = dal.Fetch(id);

            using (BypassPropertyChecks)
            {
                FromDto(dto);

                await FetchChildItemsAsync(portalChildItems);
            }
        }

        private async Task FetchChildItemsAsync(IChildDataPortal<ChildItems> childDataPortal)
        {
            ChildItems = await ChildItems.GetAsync(Id, childDataPortal);
        }

        [Insert]
        private void Insert([Inject] IWidgetDal dal)
        {
            using (BypassPropertyChecks)
            {
                var dto = ToDto();

                dal.Insert(dto);
            }

            FieldManager.UpdateChildren(this);
        }

        [Update]
        private void Update([Inject] IWidgetDal dal)
        {
            FieldManager.UpdateChildren(this);
        }

        #endregion

        #region DTO, SanitiseValues

        private void FromDto(WidgetDto dto)
        {
            Id = dto.Id;
        }

        private WidgetDto ToDto()
        {
            var dto = new WidgetDto();

            dto.Id  = Id;

            return dto;
        }

        #endregion
    }
}
