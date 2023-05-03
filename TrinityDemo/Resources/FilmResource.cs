using AbanoubNassem.Trinity.Columns;
using AbanoubNassem.Trinity.Components;
using AbanoubNassem.Trinity.Components.Interfaces;
using AbanoubNassem.Trinity.Components.TrinityColumn;
using AbanoubNassem.Trinity.Fields;
using AbanoubNassem.Trinity.Layouts;
using AbanoubNassem.Trinity.Resources;
using AbanoubNassem.Trinity.Widgets;
using FluentValidation;
using SqlKata;

namespace TrinityDemo.Resources;

public class FilmResource : TrinityResource
{
    public override string PrimaryKeyColumn => "film_id";
    protected override string Table => "film";

    public override string TitleColumn => "title";

    public override string Icon => "pi pi-camera";

    protected override void OnIndexQuery(ref Query query)
    {
        query.OrderByDesc("t.last_update");
    }

    protected override TrinityForm GetTrinityForm()
    {
        return new TrinityForm()
            .SetSchema(new List<IFormComponent>
            {
                Make<TabsLayout>(new List<Tab>()
                    {
                        Make<Tab>("Test1", new List<IFormComponent>()
                        {
                            Make<RepeaterField>("repeater")
                                .SetSchema(new List<IFormComponent>
                                {
                                    Make<TextField>("name"),
                                    Make<SelectField>("role")
                                        .SetOptions(new List<KeyValuePair<string, string>>()
                                        {
                                            new("admin", "Administrator"),
                                            new("mod", "Moderator"),
                                            new("user", "User")
                                        })
                                })
                                .SetAsCollapsible()
                        }, 1)
                    })
                    .SetAsScrollable(),


                // Make<FileUploadField>("image")
                //     .SetFileTypes()
                //     .SetMaximumFileSize(1000000)
                //     .SetAsMultiple()
                //     .SetAsImage()
                //     .SetAutoUploads()
                //     .SetCanDownload(),

                Make<DividerComponent>()
                    .SetDividerType(DividerTypes.Dashed)
                    .SetColor("green"),

                Make<GridLayout>(new List<IFormComponent>
                    {
                        Make<TextField>("title")
                            .SetValidationRules(r =>
                                r.NotNull().NotEmpty().MaximumLength(255)
                            ),
                        Make<EditorField>("description").SetOnlyOnCreate()
                            .SetValidationRules(r =>
                                r.NotNull().NotEmpty().MaximumLength(1024)
                            )
                            .SetHeaderButtons("ql-bold", "ql-italic", "ql-underline"),
                    }
                ),

                Make<GridLayout>(new List<IFormComponent>()
                {
                    Make<RangeDateTimeField>("published_at")
                        .SetAsInline()
                }).SetColumns(1),


                Make<DateTimeField>("release_year")
                    .SetLabel("Year")
                    .SetView(ViewTypes.Year)
                    .SetShowIcon()
                    .SetMaxDate(DateTime.Now)
                    .SetValidationRules(r =>
                        r.NotNull()
                            .NotEmpty()
                            .ExclusiveBetween(new DateTime(2010, 1, 1), new DateTime(DateTime.Now.Year, 1, 1))
                    )
                    .SetFillUsing(property => property?.Year),


                Make<BelongsToField>("language_id", "name", "language")
                    .SetLabel("Language")
                    .SetValidationRules(r =>
                        r.NotNull().NotEmpty()
                    ).SetAsLazy(),

                Make<PanelLayout>(new List<IFormComponent>
                    {
                        Make<NumberField<decimal>>("rental_duration")
                            .SetMin(1)
                            .SetMax(7)
                            .SetValidationRules(r =>
                                r.NotNull().NotEmpty().InclusiveBetween(1, 7)
                            ),

                        Make<NumberField<decimal>>("rental_rate")
                            .SetMinFractionDigits(2)
                            .SetCurrency("USD")
                    }
                ),

                Make<CardLayout>(new List<IFormComponent>
                    {
                        Make<SliderField>("length")
                            .SetLabel("Length").SetTooltip("Movie length in minutes."),

                        Make<NumberField<decimal>>("replacement_cost")
                            .SetMinFractionDigits(2)
                            .SetCurrency("USD")
                    }
                ),

                Make<SelectField>("rating")
                    .SetOptions(new List<KeyValuePair<string, string>>
                    {
                        new("G", "G"), new("PG", "PG"), new("PG-13", "PG-13"), new("R", "R"), new("NC-17", "NC-17")
                    })
                    .SetAsSearchable(),

                Make<MultipleSelectField>("special_features", true)
                    .SetOptions(new List<KeyValuePair<string, string>>
                    {
                        new("Trailers", "Trailers"), new("Commentaries", "Commentaries"),
                        new("Deleted Scenes", "Deleted Scenes"), new("Behind the Scenes", "Behind the Scenes")
                    })
                    .SetColumnSpan(6)
                    .SetAsSearchable(),

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
                Make<IdColumn>(PrimaryKeyColumn),

                // Make<ImageColumn>("image")
                //     .SetAlt(record => record["title"]?.ToString() ?? "")
                //     .SetAsCircular()
                //     .SetAsPreviewable()
                //     .SetAsDownloadable(),

                Make<ColorColumn>("color").SetAsHidden(),

                Make<BelongsToColumn>("language_id", "name", "language")
                    .SetLabel("Language")
                    .SetAsSortable()
                    .SetAsSearchable(),

                Make<TextColumn>("title")
                    .SetAsSearchable(
                        searchable: true,
                        globallySearchable: true,
                        caseSensitive: false,
                        searchCallback: (query, str) => { query.WhereLike("t.title", str); }
                    )
                    .SetDescription("this is a test", DescriptionPositionTypes.Above)
                    .SetLabel("this Is Title")
                    .SetAsSortable()
                    .SetAsSearchable()
                    .SetAsExportable()
                    .SetAsUrl(record => $"https://{record["title"]}.com")
                    .SetTooltip(record => $"tooltip for ${record["title"]}"),

                Make<TextColumn>("description")
                    .SetFormatUsing(record => $"<p class='text-gray-500'>{record["description"]}</p>")
                    .SetAsSearchable()
                    .SetAsHtml()
                    .SetIcon("pi pi-circle-fill"),

                Make<TextColumn>("rental_rate")
                    .SetAsCurrency("eur")
                    .SetColor("primary")
                    .SetIcon("pi pi-globe", "after")
                    .SetLabel(""),

                Make<BadgeColumn>("rating")
                    .SetOptions(
                        ("draft", "Draft", BadgeSeverityType.Info, ""),
                        ("reviewing", "Reviewing", BadgeSeverityType.Success, "pi pi-download"),
                        ("published", "Published", BadgeSeverityType.Danger, "pi-exclamation-circle")
                    )
                    .SetSize(BadgeSizeType.XLarge)
                    // .SetOptions(
                    //     ("G","G",BadgeSeverityType.Info)
                    //     // ("PG","" BadgeSeverityType.Success, "pi pi-download"),
                    //     // (null,"", BadgeSeverityType.Danger, "pi-exclamation-circle")
                    // )
                    .SetCustomFilter(
                        Make<SelectField<string>>("rating")
                            .SetOptions(new List<KeyValuePair<string, string>>
                            {
                                new("G", "G"), new("PG", "PG"), new("PG-13", "PG-13"), new("R", "R"),
                                new("NC-17", "NC-17")
                            })
                    ),

                Make<IconColumn>("test")
                    // .SetOptions(
                    //     (null, "pi pi-circle-fill"),
                    //     ("draft", "pi pi-pencil"),
                    //     ("reviewing", "pi pi-clock"),
                    //     ("published", "pi pi-check-circle")
                    // )
                    .SetAsBoolean()
                    .SetTrueValue("cool")
                    .SetFalseValue("bad")
                    .SetTooltip("this indicates value is true or false"),

                Make<TextColumn>("last_update")
                    .SetAsDateTime()
                    .SetAsTimeAgo()
                    .SetSize(SizeTypes.Sm)

                // .SetCustomFilter(
                //     Make<DateTimeField>("last_update")
                //         .SetView(DateTimeField.ViewTypes.Year)
                //     , (filters, str) => filters.Add(Make<Filter>($@" t.last_update >= STR_TO_DATE({str}, {"yyyy-MM-dd HH:mm:ss"}) "))
                // )
            })
            .SetTopWidgets(new List<ITrinityWidget>()
            {
                Make<StatsWidget>("Unique views", "192.1k")
                    .SetIcon("pi pi-shopping-cart", "red", "black")
                    .SetDescription("24 new since last visit", "green")
                    .SetChart(7, 2, 10, 3, 15, 4, 17)
                    .SetAsHidden(),
                Make<BarChartWidget>()
                    .SetLabel("Sales")
                    .SetChart(("Q1", 540), ("Q2", 325), ("Q3", 702), ("Q4", 620)),
                Make<PieChartWidget>()
                    .SetChart(("A", 540), ("B", 325), ("C", 702))
                    .SetHoverBackgroundColor("black", "green", "yellow"),
                Make<PolarAreaChartWidget>()
                    .SetChart(("A", 540), ("B", 325), ("C", 702)),
            })
            .SetBottomWidgets(new List<ITrinityWidget>()
            {
                Make<DoughnutChartWidget>()
                    .SetChart(("A", 300), ("B", 50), ("C", 100))
                    .SetHoverBackgroundColor("black", "green", "yellow"),
                Make<VerticalBarChartWidget>()
                    .SetLabels("January", "February", "March", "April", "May", "June", "July")
                    .SetDataset(new List<object>() { 65, 59, 80, 81, 56, 55, 40 }, "My First dataset")
                    .SetDataset(new List<object>() { 28, 48, 40, 19, 86, 27, 90 }, "My Second dataset"),
                Make<HorizontalBarChartWidget>()
                    .SetLabels("January", "February", "March", "April", "May", "June", "July")
                    .SetDataset(new List<object>() { 65, 59, 80, 81, 56, 55, 40 }, "My First dataset")
                    .SetDataset(new List<object>() { 28, 48, 40, 19, 86, 27, 90 }, "My Second dataset"),
                Make<StackedBarChartWidget>()
                    .SetLabels("January", "February", "March", "April", "May", "June", "July")
                    .SetDataset(new List<object>() { 50, 25, 12, 48, 90, 76, 42 }, "My First dataset")
                    .SetDataset(new List<object>() { 21, 84, 24, 75, 37, 65, 34 }, "My Second dataset")
                    .SetDataset(new List<object>() { 41, 52, 24, 74, 23, 21, 32 }, "My Third dataset"),
                Make<LineChartWidget>()
                    .SetLabels("January", "February", "March", "April", "May", "June", "July")
                    .SetDataset(new List<object>() { 65, 59, 80, 81, 56, 55, 40 }, "My First dataset")
                    .SetDataset(new List<object>() { 28, 48, 40, 19, 86, 27, 90 }, "My Second dataset")
            });
    }
}