using AbanoubNassem.Trinity.Columns;
using AbanoubNassem.Trinity.Components;
using AbanoubNassem.Trinity.Components.Interfaces;
using AbanoubNassem.Trinity.Components.TrinityAction;
using AbanoubNassem.Trinity.Components.TrinityColumn;
using AbanoubNassem.Trinity.Fields;
using AbanoubNassem.Trinity.Layouts;
using AbanoubNassem.Trinity.Resources;
using AbanoubNassem.Trinity.Widgets;
using FluentValidation;
using SqlKata;

namespace TrinityDemo.Resources;

public class CustomerResource : TrinityResource
{
    public override string PrimaryKeyColumn => "customer_id";
    protected override string Table => "customer";
    public override string TitleColumn => "first_name";
    public override string Icon => "pi pi-user";

    public override string Label => "The Customer";
    public override string PluralLabel => "The Customers";
    public override bool CanView => true;
    public override bool CanUpdate => true;

    protected override void OnIndexQuery(ref Query query)
    {
        query.OrderByDesc("t.last_update");
    }

    protected override TrinityForm GetTrinityForm()
    {
        return new TrinityForm()
            .SetSchema(new List<IFormComponent>
            {
                Make<GridLayout>(new List<IFormComponent>
                {
                    Make<TextField>("first_name")
                        .SetValidationRules(rules =>
                            rules.NotEmpty()
                                .NotNull()
                                .MinimumLength(6)
                        ),

                    Make<TextField>("last_name")
                        .SetInputType("This where you enter your first name!"),
                }),

                Make<GridLayout>(new List<IFormComponent>
                {
                    Make<TextField>("email")
                        .SetInputType("email"),

                    Make<BelongsToField>(
                            "store_id.manager_staff_id",
                            "store.staff",
                            "store_id.staff_id",
                            "store.staff",
                            "first_name"
                        )
                        .SetLabel("Manager")
                        .SetAsLazy(),
                }),

                Make<BelongsToField>("address_id", "address", "address")
                    .SetAsLazy(),
                Make<SwitchInputField>("active")
                    .SetDefaultValue(true),

                Make<DateTimeField>("create_date")
                    .SetFillUsing(_ => DateTime.Now)
                    .SetDefaultValue(DateTime.Now)
                    .SetAsHidden()
                    .SetOnlyOnCreate(),

                Make<DateTimeField>("last_update")
                    .SetFillUsing(_ => DateTime.Now)
                    .SetDefaultValue(DateTime.Now)
                    .SetAsHidden()
            });
    }


    protected override TrinityTable GetTrinityTable()
    {
        return new TrinityTable()
            .SetColumns(new List<ITrinityColumn>
            {
                Make<AggregateColumn>("store_id")
                    .Counts("store_id", "store")
                    .SetLabel("Abanoub"),

                Make<TextColumn>("first_name")
                    .SetExtraAttributes(_ => new Dictionary<string, string>
                    {
                        { "class", "bg-gray-200" }
                    }),

                Make<BelongsToColumn>(
                        "store_id.manager_staff_id",
                        "store.staff",
                        "store_id.staff_id",
                        "first_name",
                        "store.staff"
                    )
                    .SetAsSearchable(isIndividuallySearchable: true)
                    .SetLabel("Store Manager"),

                Make<TextColumn>("last_update")
                    .SetAsDateTime()
                    .SetAsTimeAgo()
                    .SetSize(SizeTypes.Sm)
            })
            .SetTopWidgets(new List<ITrinityWidget>
            {
                Make<StatsWidget>("orders", "100"),
                Make<StatsWidget>("customers", "200")
            })
            .SetBottomWidgets(new List<ITrinityWidget>
            {
                Make<StatsWidget>("customers", "200")
            })
            .SetActions(new List<ITrinityAction>
            {
                Make<TrinityAction>("View")
                    .SetSeverity(ActionSeverity.Help)
                    .SetLabel("View")
                    .SetIcon("pi pi-eye")
                    .SetAsUrl(record => $"/{Configurations.Prefix}/{Name}/edit/{record[PrimaryKeyColumn]}")
                    .SetOpenUrlInNewTab()
                    .SetRequiresConfirmation()
            })
            .SetBulkActions(new List<ITrinityAction>
            {
                // Make<TrinityAction>("download")
                //     .SetLabel("Download")
                //     .SetSeverity(ActionSeverity.Info)
                //     .SetIcon("pi pi-download")
                //     .SetRequiresConfirmation()
                //     .SetHandleActionUsing((_, _) =>
                //         TrinityAction.Download("https://localhost:7219/pdf-test.pdf", "pdf-test.pdf"))
                //     .SetForm(new TrinityForm().SetSchema(new List<IFormComponent>()
                //     {
                //         Make<GridLayout>(new List<IFormComponent>()
                //         {
                //             Make<TextField>("first_name").SetAsRequired(),
                //             Make<TextField>("last_name"),
                //         }),
                //         
                //         Make<TextField>("testing")
                //             .SetValidationRules(v => v.NotEmpty().NotNull())
                //     }))
                //     .SetSeverity(ActionSeverity.Primary)
            });
    }
}