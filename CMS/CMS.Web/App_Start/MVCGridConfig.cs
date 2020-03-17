[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(CMS.Web.MVCGridConfig), "RegisterGrids")]

namespace CMS.Web
{
    using System;
    using System.Web.Mvc;
    using System.Collections.Generic;

    using MVCGrid.Models;
    using MVCGrid.Web;
    using Common.GridModels;
    using Domain.Storage.Services;
    using Common.Enums;
    using System.Configuration;
    using Models.Rendering;
    using Common.Helpers;

    public static class MVCGridConfig
    {
        public static Dictionary<string, string> GridDictonary = new Dictionary<string, string>();

        #region RegisterGrids
        public static void RegisterGrids()
        {
            ColumnDefaults colDefauls = new ColumnDefaults()
            {
                EnableSorting = true,
            };

            GridDefaults gridDefaults = new GridDefaults
            {
                RenderingEngines = new System.Configuration.ProviderSettingsCollection
                {
                    new ProviderSettings("BootstrapRenderingEngine", "MVCGrid.Rendering.BootstrapRenderingEngine, MVCGrid"),
                    new ProviderSettings("Export", "CMS.Web.Models.Rendering.CustomCsvRenderingEngine, CMS.Web")
                },
                DefaultRenderingEngineName = "BootstrapRenderingEngine"
            };

            #endregion


            #region Student
            AddToGridDictonary<StudentGridModel>("StudentGrid");
            MVCGridDefinitionTable.Add("StudentGrid",
            new MVCGridBuilder<StudentGridModel>(gridDefaults, colDefauls)
            .AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
            .WithAuthorizationType(AuthorizationType.AllowAnonymous)
            .WithSorting(sorting: true, defaultSortColumn: "Createdon", defaultSortDirection: SortDirection.Dsc)
            .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
            .WithAdditionalQueryOptionNames("search")
            .WithFiltering(true)
            .AddColumns(cols =>
            {
                //cols.Add("Id").WithValueExpression((p, c) => c.UrlHelper.Action("detail", "demo", new { id = p.UserId }))
                //    .WithValueTemplate("<a href='{Value}'>{Model.Id}</a>", false)
                //    .WithPlainTextValueExpression(p => p.Id.ToString());
                cols.Add("RollNo").WithHeaderText("Roll No")
               .WithVisibility(true, false)
               .WithValueExpression(p => p.RollNo)
               .WithFiltering(true);
                cols.Add("FirstName").WithHeaderText("First Name")
                  .WithVisibility(true, false)
                  .WithValueExpression(p => p.FirstName)
                  .WithFiltering(true);
                cols.Add("LastName").WithHeaderText("Last Name")
                              .WithVisibility(true, true)
                              .WithValueExpression(p => p.LastName)
                              .WithFiltering(true);
                cols.Add("BoardName").WithHeaderText("Board")
                              .WithVisibility(visible: false, allowChangeVisibility: true)
                              .WithValueExpression(p => p.BoardName).WithFiltering(true);
                //    cols.Add("BatchName").WithHeaderText("Batch Name")
                //.WithVisibility(visible: false, allowChangeVisibility: true)
                //.WithValueExpression(p => p.BatchName);
                cols.Add("ClassName").WithHeaderText("Class")
                  .WithVisibility(true, true)
                  .WithValueExpression(p => p.ClassName)
                  .WithFiltering(true);
                cols.Add("SchoolName").WithHeaderText("School")
                              .WithVisibility(true, true)
                              .WithVisibility(visible: false, allowChangeVisibility: true)
                              .WithValueExpression(p => p.SchoolName);
                cols.Add("IsActive").WithHeaderText("Status")
                              .WithVisibility(true, true)
                              .WithValueExpression(p => p.IsActive ? "Active" : "Inactive");
                cols.Add("ParentContact").WithHeaderText("Parent Contact")
                              .WithVisibility(true, true)
                              .WithValueExpression(p => p.ParentContact)
                              .WithSorting(true);
                cols.Add("StudentContact").WithHeaderText("Student Contact")
                              .WithVisibility(true, true)
                              .WithValueExpression(p => p.StudentContact)
                              .WithSorting(false);
                cols.Add("Email")
                              .WithVisibility(visible: false, allowChangeVisibility: true)
                              .WithValueExpression(p => p.Email);
                cols.Add("TotalFees").WithHeaderText("Total Fees")
                              .WithVisibility(false, true)
                              .WithValueExpression(p => p.TotalFees.ToString())
                              .WithFiltering(true);
                cols.Add("BranchName").WithHeaderText("Branch")
                              .WithVisibility(true, true)
                              .WithValueExpression(p => p.BranchName)
                              .WithSorting(true);
                //cols.Add("PhotoPath").WithHeaderText("Photo")
                //              .WithVisibility(true, true)
                //              .WithValueExpression(p => p.PhotoPath != null ? p.PhotoPath.Replace("~", "..") : p.Gender == Gender.Female ? "../Content/AppImages/Female.png" : "../Content/AppImages/Male.jpg")
                //              .WithHtmlEncoding(false)
                //              .WithValueTemplate("<img src='{Value}' style='width:40px; height:40px;' />")
                //              .WithSorting(false);
                cols.Add("DOJ").WithHeaderText("DOJ")
                              .WithVisibility(true, true)
                              .WithValueExpression(p => p.DOJ.ToISTTimeZone().ToString("dd/MM/yyyy"))
                              .WithFiltering(true);
                //    cols.Add("Address")
                //.WithVisibility(visible: false, allowChangeVisibility: true)
                //.WithValueExpression(p => p.Address);
                //cols.Add("pin")
                //    .WithVisibility(visible: false, allowChangeVisibility: true)
                //    .WithValueExpression(p => p.pin);
                //cols.Add("DOB")
                //    .WithVisibility(visible: false, allowChangeVisibility: true)
                //    .WithValueExpression(p => p.DOB.ToString("dd/MM/yyyy"));
                //cols.Add("PickAndDrop")
                //    .WithVisibility(visible: false, allowChangeVisibility: true)
                //    .WithValueExpression(p => p.PickAndDrop ? "Yes" : "No");
                //cols.Add("BloodGroup")
                //    .WithVisibility(visible: false, allowChangeVisibility: true)
                //    .WithValueExpression(p => p.BloodGroup.ToString());
                //cols.Add("BoardNumber")
                //    .WithVisibility(visible: false, allowChangeVisibility: true)
                //    .WithValueExpression(p => p.BoardNumber);
                //cols.Add("VANFee")
                //    .WithVisibility(visible: false, allowChangeVisibility: true)
                //    .WithValueExpression(p => p.VANFee.ToString());
                //cols.Add("VANArea")
                //    .WithVisibility(visible: false, allowChangeVisibility: true)
                //    .WithValueExpression(p => p.VANArea);
                //cols.Add("Discount")
                //    .WithVisibility(visible: false, allowChangeVisibility: true)
                //    .WithValueExpression(p => p.Discount.ToString());
                cols.Add("FinalFees")
                  .WithVisibility(visible: true, allowChangeVisibility: true)
                  .WithValueExpression(p => p.FinalFees.ToString())
                  .WithFiltering(true);
                cols.Add("PunchId")
                              .WithVisibility(visible: false, allowChangeVisibility: true)
                              .WithValueExpression(p => p.PunchId.ToString())
                               .WithFiltering(true);
                cols.Add("IsWhatsApp")
                              .WithVisibility(visible: false, allowChangeVisibility: true)
                              .WithValueExpression(p => p.IsWhatsApp ? "Yes" : "No");
                cols.Add("Createdon")
                              .WithVisibility(visible: false, allowChangeVisibility: false)
                              .WithValueExpression(p => p.Createdon.ToISTTimeZone().ToString());
                cols.Add("Action").WithColumnName("Action")
                              .WithVisibility(true, false)
                              .WithSorting(false)
                              .WithHeaderText("Action")
                              .WithHtmlEncoding(false)
                              .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Edit", "Student", new { id = p.UserId }) + "'>Edit</a> | <a href='" + c.UrlHelper.Action("Details", "Student", new { id = p.UserId }) + "'>Details</a>")
                              .WithValueTemplate("{Value}");
            })
            //.WithAdditionalSetting(MVCGrid.Rendering.BootstrapRenderingEngine.SettingNameTableClass, "notreal") // Example of changing table css class
            .WithRetrieveDataMethod((context) =>
            {
                var options = context.QueryOptions;
                int totalRecords;
                int userId = Convert.ToInt32(options.GetPageParameterString("userRole"));
                var repo = DependencyResolver.Current.GetService<IStudentService>();
                string globalSearch = options.GetAdditionalQueryOptionString("search");
                string sortColumn = options.GetSortColumnData<string>();
                var items = repo.GetData(out totalRecords, Convert.ToInt16(options.GetFilterString("ClassName")), options.GetFilterString("FirstName"), userId,
                            options.GetFilterString("LastName"), options.GetLimitOffset(), options.GetLimitRowcount(),
                            sortColumn, options.SortDirection == SortDirection.Dsc);
                return new QueryResult<StudentGridModel>()
                {
                    Items = items,
                    TotalRecords = totalRecords
                };
            })
            );
            #endregion

            #region Board
            AddToGridDictonary<BranchGridModel>("BoardGrid");
            MVCGridDefinitionTable.Add("BoardGrid",
                  new MVCGridBuilder<BoardGridModel>(gridDefaults, colDefauls).AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
                  .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                  .WithSorting(sorting: true, defaultSortColumn: "CreatedOn", defaultSortDirection: SortDirection.Dsc)
                  .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                  .WithAdditionalQueryOptionNames("search")
                  .WithFiltering(true)
                  .AddColumns(cols =>
                  {
                      cols.Add("BoardName").WithHeaderText("Board Name")
                         .WithVisibility(true, false)
                         .WithValueExpression(p => p.BoardName)
                         .WithFiltering(true);
                      cols.Add("Createdon")
                              .WithVisibility(visible: false, allowChangeVisibility: false)
                               .WithValueExpression(p => p.CreatedOn.ToISTTimeZone().ToString());
                      cols.Add("Action").WithColumnName("Action")
                                         .WithVisibility(true, false)
                                            .WithSorting(false)
                                            .WithHeaderText("Action")
                                            .WithHtmlEncoding(false)
                                            .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Edit", "Board", new { id = p.BoardId }) + "'>Edit</a> | <a href='" + c.UrlHelper.Action("Delete", "Board", new { id = p.BoardId }) + "'>Delete</a>")
                                            .WithValueTemplate("{Value}");
                  })
                     .WithRetrieveDataMethod((context) =>
                  {
                      var options = context.QueryOptions;
                      int totalRecords;
                      int userId = Convert.ToInt32(options.GetPageParameterString("userRole"));
                      var repo = DependencyResolver.Current.GetService<IBoardService>();
                      string globalSearch = options.GetAdditionalQueryOptionString("search");
                      string sortColumn = options.GetSortColumnData<string>();
                      var items = repo.GetBoardDataByClientId(out totalRecords, options.GetFilterString("BoardName"), userId,
                                      options.GetLimitOffset(), options.GetLimitRowcount(),
                          sortColumn, options.SortDirection == SortDirection.Dsc);
                      return new QueryResult<BoardGridModel>()
                      {
                          Items = items,
                          TotalRecords = totalRecords
                      };
                  })
              );
            #endregion

            #region branch
            AddToGridDictonary<BranchGridModel>("BranchGrid"); //Must Add to exclude columns from csv exports
            MVCGridDefinitionTable.Add("BranchGrid",
                  new MVCGridBuilder<BranchGridModel>(gridDefaults, colDefauls).AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
                  .AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine)) //Must Add to exclude columns from csv exports
                  .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                  .WithSorting(sorting: true, defaultSortColumn: "CreatedOn", defaultSortDirection: SortDirection.Dsc)
                  .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                  .WithAdditionalQueryOptionNames("search")
                  .WithFiltering(true)
                  .AddColumns(cols =>
                  {
                      cols.Add("BranchName").WithHeaderText("Branch Name")
                          .WithVisibility(true, false)
                          .WithValueExpression(p => p.BranchName)
                          .WithFiltering(true);
                      cols.Add("Address").WithHeaderText("Address")
                          .WithVisibility(true, true)
                          .WithSorting(false)
                          .WithValueExpression(p => p.Address);
                      cols.Add("Createdon")
                              .WithVisibility(visible: false, allowChangeVisibility: false)
                               .WithValueExpression(p => p.CreatedOn.ToString());
                      cols.Add("Action").WithColumnName("Action")
                                           .WithVisibility(true, false)
                                              .WithSorting(false)
                                              .WithHeaderText("Action")
                                              .WithHtmlEncoding(false)
                                              .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Edit", "Branch", new { id = p.BranchId }) + "'>Edit</a> | <a href='" + c.UrlHelper.Action("Delete", "Branch", new { id = p.BranchId }) + "'>Delete</a>")
                                              .WithValueTemplate("{Value}");
                  })
                  .WithRetrieveDataMethod((context) =>
                  {
                      var options = context.QueryOptions;
                      int totalRecords;
                      int userId = Convert.ToInt32(options.GetPageParameterString("userRole"));
                      var repo = DependencyResolver.Current.GetService<IBranchService>();
                      string globalSearch = options.GetAdditionalQueryOptionString("search");
                      string sortColumn = options.GetSortColumnData<string>();
                      var items = repo.GetBranchDataByClientId(out totalRecords, options.GetFilterString("BranchName"), userId,
                                      options.GetLimitOffset(), options.GetLimitRowcount(),
                          sortColumn, options.SortDirection == SortDirection.Dsc);
                      return new QueryResult<BranchGridModel>()
                      {
                          Items = items,
                          TotalRecords = totalRecords
                      };
                  })
              );
            #endregion


            #region client
            AddToGridDictonary<ClientGridModel>("ClientGrid"); //Must Add to exclude columns from csv exports
            MVCGridDefinitionTable.Add("ClientGrid",
                  new MVCGridBuilder<ClientGridModel>(gridDefaults, colDefauls).AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
                  .AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine)) //Must Add to exclude columns from csv exports
                  .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                  .WithSorting(sorting: true, defaultSortColumn: "CreatedOn", defaultSortDirection: SortDirection.Dsc)
                  .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                  .WithAdditionalQueryOptionNames("search")
                  .WithFiltering(true)
                  .AddColumns(cols =>
                  {
                      cols.Add("ClientName").WithHeaderText("Client Name")
                          .WithVisibility(true, false)
                          .WithValueExpression(p => p.ClientName)
                          .WithFiltering(true);
                      cols.Add("Address").WithHeaderText("Address")
                          .WithVisibility(true, true)
                          .WithSorting(false)
                          .WithValueExpression(p => p.Address);
                      cols.Add("Createdon")
                              .WithVisibility(visible: false, allowChangeVisibility: false)
                               .WithValueExpression(p => p.CreatedOn.ToString());
                      cols.Add("Action").WithColumnName("Action")
                                           .WithVisibility(true, false)
                                              .WithSorting(false)
                                              .WithHeaderText("Action")
                                              .WithHtmlEncoding(false)
                                              .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Edit", "Client", new { id = p.ClientId }) + "'>Edit</a> | <a href='" + c.UrlHelper.Action("Delete", "Client", new { id = p.ClientId }) + "'>Delete</a>")
                                              .WithValueTemplate("{Value}");
                  })
                  .WithRetrieveDataMethod((context) =>
                  {
                      var options = context.QueryOptions;
                      int totalRecords;
                      var repo = DependencyResolver.Current.GetService<IClientService>();
                      string globalSearch = options.GetAdditionalQueryOptionString("search");
                      string sortColumn = options.GetSortColumnData<string>();
                      var items = repo.GetClientData(out totalRecords, options.GetFilterString("ClientName"),
                                      options.GetLimitOffset(), options.GetLimitRowcount(),
                          sortColumn, options.SortDirection == SortDirection.Dsc);
                      return new QueryResult<ClientGridModel>()
                      {
                          Items = items,
                          TotalRecords = totalRecords
                      };
                  })
              );
            #endregion

            #region school
            AddToGridDictonary<SchoolGridModel>("SchoolGrid");
            MVCGridDefinitionTable.Add("SchoolGrid",
                 new MVCGridBuilder<SchoolGridModel>(gridDefaults, colDefauls).AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
                 .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                 .WithSorting(sorting: true, defaultSortColumn: "CreatedOn", defaultSortDirection: SortDirection.Dsc)
                 .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                 .WithAdditionalQueryOptionNames("search")
                 .WithFiltering(true)
                 .AddColumns(cols =>
                 {
                     cols.Add("SchoolName").WithHeaderText("School Name")
                         .WithVisibility(true, false)
                         .WithValueExpression(p => p.SchoolName)
                         .WithFiltering(true);
                     cols.Add("CenterNumber").WithHeaderText("CenterNumber")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.CenterNumber)
                          .WithFiltering(true);
                     cols.Add("Action").WithColumnName("Action")
                                          .WithVisibility(true, false)
                                             .WithSorting(false)
                                             .WithHeaderText("Action")
                                             .WithHtmlEncoding(false)
                                             .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Edit", "School", new { id = p.SchoolId }) + "'>Edit</a> | <a href='" + c.UrlHelper.Action("Delete", "School", new { id = p.SchoolId }) + "'>Delete</a>")
                                             .WithValueTemplate("{Value}");
                     cols.Add("Createdon")
                              .WithVisibility(visible: false, allowChangeVisibility: false)
                               .WithValueExpression(p => p.CreatedOn.ToString());
                 })
                 .WithRetrieveDataMethod((context) =>
                 {
                     var options = context.QueryOptions;
                     int totalRecords;
                     int userId = Convert.ToInt32(options.GetPageParameterString("userRole"));
                     var repo = DependencyResolver.Current.GetService<ISchoolService>();
                     string globalSearch = options.GetAdditionalQueryOptionString("search");
                     string sortColumn = options.GetSortColumnData<string>();
                     var items = repo.GetSchoolDataByClientId(out totalRecords, options.GetFilterString("SchoolName"), userId,
                                     options.GetLimitOffset(), options.GetLimitRowcount(),
                         sortColumn, options.SortDirection == SortDirection.Dsc);
                     return new QueryResult<SchoolGridModel>()
                     {
                         Items = items,
                         TotalRecords = totalRecords
                     };
                 })
             );
            #endregion

            #region class
            AddToGridDictonary<ClassGridModel>("ClassGrid");
            MVCGridDefinitionTable.Add("ClassGrid",
                new MVCGridBuilder<ClassGridModel>(gridDefaults, colDefauls).AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
                .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                .WithSorting(sorting: true, defaultSortColumn: "CreatedOn", defaultSortDirection: SortDirection.Dsc)
                .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                .WithAdditionalQueryOptionNames("search")
                .WithFiltering(true)
                .AddColumns(cols =>
                {
                    cols.Add("ClassName").WithHeaderText("Class Name")
                        .WithVisibility(true, false)
                        .WithValueExpression(p => p.ClassName)
                        .WithFiltering(true);
                    cols.Add("Createdon")
                              .WithVisibility(visible: false, allowChangeVisibility: false)
                               .WithValueExpression(p => p.CreatedOn.ToString());
                    cols.Add("Action").WithColumnName("Action")
                                      .WithVisibility(true, false)
                                         .WithSorting(false)
                                         .WithHeaderText("Action")
                                         .WithHtmlEncoding(false)
                                         .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Edit", "Class", new { id = p.ClassId }) + "'>Edit</a> | <a href='" + c.UrlHelper.Action("Delete", "Class", new { id = p.ClassId }) + "'>Delete</a>")
                                         .WithValueTemplate("{Value}");
                })
                .WithRetrieveDataMethod((context) =>
                {
                    var options = context.QueryOptions;
                    int totalRecords;
                    int userId = Convert.ToInt32(options.GetPageParameterString("userRole"));
                    var repo = DependencyResolver.Current.GetService<IClassService>();
                    string globalSearch = options.GetAdditionalQueryOptionString("search");
                    string sortColumn = options.GetSortColumnData<string>();
                    var items = repo.GetClassDataByClientId(out totalRecords, options.GetFilterString("ClassName"), userId,
                                    options.GetLimitOffset(), options.GetLimitRowcount(),
                        sortColumn, options.SortDirection == SortDirection.Dsc);
                    return new QueryResult<ClassGridModel>()
                    {
                        Items = items,
                        TotalRecords = totalRecords
                    };
                })
            );
            #endregion

            #region Subject
            AddToGridDictonary<SubjectGridModel>("SubjectGrid");
             MVCGridDefinitionTable.Add("SubjectGrid",
         new MVCGridBuilder<SubjectGridModel>(gridDefaults, colDefauls).AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
         .WithAuthorizationType(AuthorizationType.AllowAnonymous)
         .WithSorting(sorting: true, defaultSortColumn: "CreatedOn", defaultSortDirection: SortDirection.Dsc)
         .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
         .WithAdditionalQueryOptionNames("search")
         .WithFiltering(true)
         .AddColumns(cols =>
         {
             cols.Add("SubjectName").WithHeaderText("Subject Name")
                 .WithVisibility(true, false)
                 .WithValueExpression(p => p.SubjectName)
                 .WithFiltering(true);
             cols.Add("ClassName").WithHeaderText("Class Name")
              .WithVisibility(true, true)
              .WithValueExpression(p => p.ClassName)
              .WithFiltering(true);
             cols.Add("Createdon")
                              .WithVisibility(visible: false, allowChangeVisibility: false)
                               .WithValueExpression(p => p.CreatedOn.ToString());
             cols.Add("Action").WithColumnName("Action")
                                 .WithVisibility(true, false)
                                    .WithSorting(false)
                                    .WithHeaderText("Action")
                                    .WithHtmlEncoding(false)
                                    .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Edit", "Subject", new { id = p.SubjectId }) + "'>Edit</a> | <a href='" + c.UrlHelper.Action("Delete", "Subject", new { id = p.SubjectId }) + "'>Delete</a>")
                                    .WithValueTemplate("{Value}");
         })
         .WithRetrieveDataMethod((context) =>
         {
             var options = context.QueryOptions;
             int totalRecords;
             int userId = Convert.ToInt32(options.GetPageParameterString("userRole"));
             var repo = DependencyResolver.Current.GetService<ISubjectService>();
             string globalSearch = options.GetAdditionalQueryOptionString("search");
             string sortColumn = options.GetSortColumnData<string>();
             var items = repo.GetSubjectDataByClientId(out totalRecords, options.GetFilterString("SubjectName"), Convert.ToInt16(options.GetFilterString("ClassName")), userId,
                             options.GetLimitOffset(), options.GetLimitRowcount(),
                 sortColumn, options.SortDirection == SortDirection.Dsc);
             return new QueryResult<SubjectGridModel>()
             {
                 Items = items,
                 TotalRecords = totalRecords
             };
         })
     );
            #endregion

            #region Batch
            AddToGridDictonary<BatchGridModel>("BatchGrid");
            MVCGridDefinitionTable.Add("BatchGrid",
            new MVCGridBuilder<BatchGridModel>(gridDefaults, colDefauls).AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
            .WithAuthorizationType(AuthorizationType.AllowAnonymous)
            .WithSorting(sorting: true, defaultSortColumn: "Createdon", defaultSortDirection: SortDirection.Dsc)
            .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
            .WithAdditionalQueryOptionNames("search")
            .WithFiltering(true)
            .WithPageParameterNames("userRole")
            .AddColumns(cols =>
            {
                cols.Add("BatchName").WithHeaderText("Batch Name")
                .WithVisibility(true, false)
                .WithValueExpression(p => p.BatchName)
                .WithFiltering(true);
                cols.Add("ClassName").WithHeaderText("Class Name")
               .WithVisibility(true, false)
               .WithValueExpression(p => p.ClassName)
               .WithFiltering(true);
                cols.Add("InTime").WithHeaderText("In Time")
                .WithVisibility(true, true)
                .WithSorting(false)
                .WithValueExpression(p => p.InTime.ToString("hh:mm:ss tt") == "12:00:00 AM" ? "-" : p.InTime.ToString("hh:mm tt"));
                cols.Add("OutTime").WithHeaderText("Out Time")
                .WithVisibility(true, true)
                  .WithSorting(false)
                .WithValueExpression(p => p.OutTime.ToString("hh:mm:ss tt") == "12:00:00 AM" ? "-" : p.OutTime.ToString("hh:mm tt"));
                cols.Add("Createdon")
                              .WithVisibility(visible: false, allowChangeVisibility: false)
                               .WithValueExpression(p => p.CreatedOn.ToString());
                cols.Add("Action").WithColumnName("Action")
                               .WithVisibility(true, false)
                                  .WithSorting(false)
                                  .WithHeaderText("Action")
                                  .WithHtmlEncoding(false)
                                  .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Edit", "Batch", new { id = p.BatchId }) + "'>Edit</a> | <a href='" + c.UrlHelper.Action("Delete", "Batch", new { id = p.BatchId }) + "'>Delete</a>")
                                  .WithValueTemplate("{Value}");
            })
            .WithRetrieveDataMethod((context) =>
            {
                var options = context.QueryOptions;
                int totalRecords;
                int userId = Convert.ToInt32(options.GetPageParameterString("userRole"));
                var repo = DependencyResolver.Current.GetService<IBatchService>();
                string globalSearch = options.GetAdditionalQueryOptionString("search");
                string sortColumn = options.GetSortColumnData<string>();
                var items = repo.GetBatchDataByClientId(out totalRecords, Convert.ToInt16(options.GetFilterString("ClassName")), userId,
                                options.GetLimitOffset(), options.GetLimitRowcount(),
                    sortColumn, options.SortDirection == SortDirection.Dsc);
                return new QueryResult<BatchGridModel>()
                {
                    Items = items,
                    TotalRecords = totalRecords
                };
            })
        );
            #endregion

            #region chapter
            AddToGridDictonary<ChapterGridModel>("ChapterGrid");
            MVCGridDefinitionTable.Add("ChapterGrid",
            new MVCGridBuilder<ChapterGridModel>(gridDefaults, colDefauls).AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
            .WithAuthorizationType(AuthorizationType.AllowAnonymous)
            .WithSorting(sorting: true, defaultSortColumn: "CreatedOn", defaultSortDirection: SortDirection.Dsc)
            .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
            .WithAdditionalQueryOptionNames("search")
            .WithFiltering(true)
            .AddColumns(cols =>
            {
                cols.Add("ChapterId").WithHeaderText("Chapter Id")
                .WithVisibility(false, false)
                .WithValueExpression(p => p.ChapterId.ToString())
                .WithFiltering(true);
                cols.Add("ChapterName").WithHeaderText("Chapter Name")
                .WithVisibility(true, false)
                .WithValueExpression(p => p.ChapterName)
                .WithFiltering(true);
                cols.Add("ClassName").WithHeaderText("Class Name")
                   .WithVisibility(true, true)
                   .WithValueExpression(p => p.ClassName)
                   .WithFiltering(true);
                cols.Add("SubjectName").WithHeaderText("Subject Name")
                    .WithVisibility(true, true)
                    .WithValueExpression(p => p.SubjectName)
                    .WithFiltering(true);
                cols.Add("SubjectId").WithHeaderText("Subject Id")
                     .WithVisibility(false, false)
                     .WithValueExpression(p => p.SubjectId.ToString())
                     .WithFiltering(true);
                cols.Add("Weightage").WithHeaderText("Weightage")
                .WithVisibility(true, true)
                .WithValueExpression(p => p.Weightage.ToString())
                .WithFiltering(true);
                cols.Add("Createdon")
                              .WithVisibility(visible: false, allowChangeVisibility: false)
                               .WithValueExpression(p => p.CreatedOn.ToString());
                cols.Add("Action").WithColumnName("Action")
                              .WithVisibility(true, false)
                                 .WithSorting(false)
                                 .WithHeaderText("Action")
                                 .WithHtmlEncoding(false)
                                 .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Edit", "Chapter", new { id = p.ChapterId }) + "'>Edit</a> | <a href='" + c.UrlHelper.Action("Delete", "Chapter", new { id = p.ChapterId }) + "'>Delete</a>")
                                 .WithValueTemplate("{Value}");
            })
            .WithRetrieveDataMethod((context) =>
            {
                var options = context.QueryOptions;
                int totalRecords;
                int userId = Convert.ToInt32(options.GetPageParameterString("userRole"));
                var repo = DependencyResolver.Current.GetService<IChapterService>();
                string globalSearch = options.GetAdditionalQueryOptionString("search");
                string sortColumn = options.GetSortColumnData<string>();
                var items = repo.GetChapterDataByClientId(out totalRecords, Convert.ToInt16(options.GetFilterString("ClassName")), Convert.ToInt16(options.GetFilterString("SubjectName")), userId,
                                options.GetLimitOffset(), options.GetLimitRowcount(),
                    sortColumn, options.SortDirection == SortDirection.Dsc);
                return new QueryResult<ChapterGridModel>()
                {
                    Items = items,
                    TotalRecords = totalRecords
                };
            })
        );
            #endregion

            #region Installment
            AddToGridDictonary<InstallmentGridModel>("InstallmentGrid");
            MVCGridDefinitionTable.Add("InstallmentGrid",
           new MVCGridBuilder<InstallmentGridModel>(gridDefaults, colDefauls).AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
           .WithAuthorizationType(AuthorizationType.AllowAnonymous)
           .WithSorting(sorting: true, defaultSortColumn: "CreatedOn", defaultSortDirection: SortDirection.Dsc)
           .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
           .WithAdditionalQueryOptionNames("search")
           .WithFiltering(true)
           .WithPageParameterNames("userRole")
           .AddColumns(cols =>
           {
               cols.Add("BranchName")
              .WithVisibility(visible: false, allowChangeVisibility: true)
              .WithValueExpression(p => p.BranchName)
              .WithFiltering(true);
               cols.Add("InstallmentId").WithHeaderText("Installment Id")
                   .WithVisibility(false, false)
                   .WithValueExpression(p => p.InstallmentId.ToString())
                   .WithFiltering(true);
               cols.Add("UserId").WithHeaderText("UserId")
                    .WithVisibility(false, false)
                    .WithValueExpression(p => p.UserId)
                    .WithFiltering(true);
               cols.Add("ClassName").WithHeaderText("Class Name")
                   .WithVisibility(true, false)
                   .WithValueExpression(p => p.ClassName)
                   .WithFiltering(true);
               cols.Add("StudentFirstName").WithHeaderText("Student Name")
                   .WithVisibility(true, true)
                   .WithValueExpression(p => p.StudentFirstName + " " + p.StudentMiddleName + " " + p.StudentLastName)
                   .WithFiltering(true);
               cols.Add("CreatedOn").WithHeaderText("Date")
                   .WithVisibility(true, true)
                   .WithValueExpression(p => p.CreatedOn.ToString("dd/MM/yyyy"))
                   .WithFiltering(true);
               cols.Add("TotalFee").WithHeaderText("Received/Total Fee")
                    .WithVisibility(true, true)
                    .WithSorting(false)
                    .WithValueExpression(p => p.ReceivedFee + "/" + p.TotalFee);
               cols.Add("Payment").WithHeaderText("Last Paid")
                    .WithVisibility(true, true)
                    .WithSorting(false)
                    .WithValueExpression(p => p.Payment.ToString())
                    .WithFiltering(true);
               cols.Add("RemainingFee").WithHeaderText("Remaining Fee")
                    .WithVisibility(true, true)
                    .WithSorting(false)
                    .WithValueExpression(p => p.RemainingFee.ToString())
                    .WithFiltering(true);
               cols.Add("ReceiptNumber").WithHeaderText("Receipt No.")
                  .WithVisibility(true, true)
                  .WithValueExpression(p => p.ReceiptNumber)
                  .WithFiltering(true);
               cols.Add("ReceiptBookNumber").WithHeaderText("Receipt Book No.")
                   .WithVisibility(true, true)
                   .WithValueExpression(p => p.ReceiptBookNumber)
                   .WithFiltering(true);
               cols.Add("Installment No.")
                    .WithVisibility(visible: false, allowChangeVisibility: false)
                    .WithValueExpression(p => p.InstallmentNo.ToString());
               cols.Add("Action").WithColumnName("Action")
                    .WithVisibility(true, false)
                    .WithSorting(false)
                    .WithHeaderText("Action")
                    .WithHtmlEncoding(false)
                    .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Edit", "Installment", new { id = p.InstallmentId }) + "'>Edit</a>")
                    .WithValueTemplate("{Value}");
           })
           .WithRetrieveDataMethod((context) =>
           {
               var options = context.QueryOptions;
               int totalRecords;
               int userId = Convert.ToInt16(options.GetPageParameterString("userRole"));
               var repo = DependencyResolver.Current.GetService<IInstallmentService>();
               string globalSearch = options.GetAdditionalQueryOptionString("search");
               string sortColumn = options.GetSortColumnData<string>();
               var items = repo.GetInstallmentData(out totalRecords, Convert.ToInt16(options.GetFilterString("ClassName")), (options.GetFilterString("UserId")), userId,
                               options.GetLimitOffset(), options.GetLimitRowcount(),
                   sortColumn, options.SortDirection == SortDirection.Dsc);
               return new QueryResult<InstallmentGridModel>()
               {
                   Items = items,
                   TotalRecords = totalRecords
               };
           })
       );
            #endregion

            #region teacher
            AddToGridDictonary<TeacherGridModel>("TeacherGrid");
            MVCGridDefinitionTable.Add("TeacherGrid",
                   new MVCGridBuilder<TeacherGridModel>(gridDefaults, colDefauls).AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
                   .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                   .WithSorting(sorting: true, defaultSortColumn: "Createdon", defaultSortDirection: SortDirection.Dsc)
                   .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                   .WithAdditionalQueryOptionNames("search")
                   .WithFiltering(true)
                   .WithPageParameterNames("userRole")
                   .AddColumns(cols =>
                   {
                       //cols.Add("Id").WithValueExpression((p, c) => c.UrlHelper.Action("detail", "demo", new { id = p.UserId }))
                       //    .WithValueTemplate("<a href='{Value}'>{Model.Id}</a>", false)
                       //    .WithPlainTextValueExpression(p => p.Id.ToString());
                       cols.Add("Qualification").WithHeaderText("Qualification")
                              .WithVisibility(true, false)
                              .WithValueExpression(p => p.Qualification)
                              .WithFiltering(true);
                       cols.Add("FirstName").WithHeaderText("First Name")
                              .WithVisibility(true, false)
                              .WithValueExpression(p => p.FirstName)
                              .WithFiltering(true);
                       cols.Add("LastName").WithHeaderText("Last Name")
                           .WithVisibility(true, false)
                           .WithValueExpression(p => p.LastName)
                           .WithFiltering(true);
                       cols.Add("BranchName").WithHeaderText("Branch")
                          .WithVisibility(true, true)
                          .WithValueExpression(p => p.BranchName)
                          .WithFiltering(true);
                       cols.Add("Email")
                           .WithVisibility(true, true)
                           .WithValueExpression(p => p.Email)
                             .WithFiltering(false);
                       cols.Add("ContactNo").WithHeaderText("Contact No")
                          .WithVisibility(true, true)
                          .WithValueExpression(p => p.ContactNo)
                            .WithFiltering(false);
                       cols.Add("Description").WithHeaderText("Description")
                          .WithVisibility(true, true)
                          .WithValueExpression(p => p.Description)
                            .WithFiltering(false);
                       cols.Add("IsActive").WithHeaderText("Status")
                          .WithVisibility(true, true)
                          .WithValueExpression(p => p.IsActive ? "Active" : "Inactive")
                           .WithFiltering(true);
                       cols.Add("Createdon")
                              .WithVisibility(visible: false, allowChangeVisibility: false)
                               .WithVisibility(false, true)
                               .WithValueExpression(p => p.CreatedOn.ToISTTimeZone().ToString("dd/MM/yyyy"));
                       cols.Add("Action").WithColumnName("Action")
                            .WithVisibility(true, false)
                               .WithSorting(false)
                               .WithHeaderText("Action")
                               .WithHtmlEncoding(false)
                               .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Edit", "Teacher", new { id = p.UserId }) + "'>Edit</a> | <a href='" + c.UrlHelper.Action("Details", "Teacher", new { id = p.UserId }) + "'>Details</a>")
                               .WithValueTemplate("{Value}");
                   })
                   //.WithAdditionalSetting(MVCGrid.Rendering.BootstrapRenderingEngine.SettingNameTableClass, "notreal") // Example of changing table css class
                   .WithRetrieveDataMethod((context) =>
                   {
                       var options = context.QueryOptions;
                       int userId = Convert.ToInt16(options.GetPageParameterString("userRole"));
                       int totalRecords;
                       var repo = DependencyResolver.Current.GetService<ITeacherService>();
                       string globalSearch = options.GetAdditionalQueryOptionString("search");
                       string sortColumn = options.GetSortColumnData<string>();
                       var items = repo.GetTeacherData(out totalRecords, options.GetFilterString("FirstName"),
                                       options.GetFilterString("LastName"), userId, options.GetLimitOffset(), options.GetLimitRowcount(),
                           sortColumn, options.SortDirection == SortDirection.Dsc);
                       return new QueryResult<TeacherGridModel>()
                       {
                           Items = items,
                           TotalRecords = totalRecords
                       };
                   })
               );
            #endregion

            #region BranchAdmin
            AddToGridDictonary<BranchAdminGridModel>("BranchAdminGrid");
            MVCGridDefinitionTable.Add("BranchAdminGrid",
                   new MVCGridBuilder<BranchAdminGridModel>(gridDefaults, colDefauls).AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
                   .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                   .WithSorting(sorting: true, defaultSortColumn: "CreatedOn", defaultSortDirection: SortDirection.Dsc)
                   .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                   .WithAdditionalQueryOptionNames("search")
                   .WithFiltering(true)
                   .AddColumns(cols =>
                   {
                       //cols.Add("Id").WithValueExpression((p, c) => c.UrlHelper.Action("detail", "demo", new { id = p.UserId }))
                       //    .WithValueTemplate("<a href='{Value}'>{Model.Id}</a>", false)
                       //    .WithPlainTextValueExpression(p => p.Id.ToString());
                       cols.Add("BranchAdminName").WithHeaderText("Name")
                              .WithVisibility(true, false)
                              .WithValueExpression(p => p.Name)
                              .WithFiltering(true);
                       cols.Add("BranchName").WithHeaderText("Branch")
                          .WithVisibility(true, true)
                          .WithValueExpression(p => p.BranchName)
                          .WithFiltering(true);
                       cols.Add("Email")
                           .WithVisibility(true, true)
                           .WithValueExpression(p => p.Email)
                               .WithFiltering(false);
                       cols.Add("ContactNo").WithHeaderText("Contact No")
                          .WithVisibility(true, true)
                          .WithValueExpression(p => p.ContactNo)
                           .WithFiltering(false);
                       cols.Add("IsActive").WithHeaderText("Status")
                          .WithVisibility(true, true)
                          .WithValueExpression(p => p.IsActive ? "Active" : "Inactive")
                          .WithFiltering(true);
                       cols.Add("Createdon")
                              .WithVisibility(visible: false, allowChangeVisibility: false)
                               .WithValueExpression(p => p.CreatedOn.ToString());
                       cols.Add("Action").WithColumnName("Action")
                           .WithVisibility(true, false)
                              .WithSorting(false)
                              .WithHeaderText("Action")
                              .WithHtmlEncoding(false)
                              .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Edit", "BranchAdmin", new { id = p.UserId }) + "'>Edit</a> | <a href='" + c.UrlHelper.Action("Details", "BranchAdmin", new { id = p.UserId }) + "'>Details</a>")
                              .WithValueTemplate("{Value}");
                   })
                   //.WithAdditionalSetting(MVCGrid.Rendering.BootstrapRenderingEngine.SettingNameTableClass, "notreal") // Example of changing table css class
                   .WithRetrieveDataMethod((context) =>
                   {
                       var options = context.QueryOptions;
                       int totalRecords;
                       var repo = DependencyResolver.Current.GetService<IBranchAdminService>();
                       string globalSearch = options.GetAdditionalQueryOptionString("search");
                       string sortColumn = options.GetSortColumnData<string>();
                       var items = repo.GetBranchAdminData(out totalRecords, options.GetFilterString("BranchAdminName"), options.GetLimitOffset(), options.GetLimitRowcount(),
                           sortColumn, options.SortDirection == SortDirection.Dsc);
                       return new QueryResult<BranchAdminGridModel>()
                       {
                           Items = items,
                           TotalRecords = totalRecords
                       };
                   })
               );
            #endregion

            #region Configure
            AddToGridDictonary<ConfigureGridModel>("ConfigureGrid");
            MVCGridDefinitionTable.Add("ConfigureGrid",
                   new MVCGridBuilder<ConfigureGridModel>(gridDefaults, colDefauls).AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
                   .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                   .WithSorting(sorting: true, defaultSortColumn: "CreatedOn", defaultSortDirection: SortDirection.Dsc)
                   .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                   .WithAdditionalQueryOptionNames("search")
                   .WithFiltering(true)
                   .AddColumns(cols =>
                   {
                       //cols.Add("Id").WithValueExpression((p, c) => c.UrlHelper.Action("detail", "demo", new { id = p.UserId }))
                       //    .WithValueTemplate("<a href='{Value}'>{Model.Id}</a>", false)
                       //    .WithPlainTextValueExpression(p => p.Id.ToString());
                       cols.Add("ConfigureName").WithHeaderText("Name")
                              .WithVisibility(true, false)
                              .WithValueExpression(p => p.name)
                              .WithFiltering(true);
                       cols.Add("Aboutus").WithHeaderText("Aboutus")
                          .WithVisibility(true, true)
                          .WithValueExpression(p => p.aboutus)
                          .WithFiltering(true);
                       cols.Add("Address")
                           .WithVisibility(true, true)
                           .WithValueExpression(p => p.address)
                               .WithFiltering(false);
                       cols.Add("Email_Id").WithHeaderText("Email Id")
                          .WithVisibility(true, true)
                          .WithValueExpression(p => p.email_id)
                           .WithFiltering(false);
                       cols.Add("EmailPassword").WithHeaderText("Email Password")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.emailpassword)
                          .WithFiltering(false);
                       cols.Add("Username").WithHeaderText("User Name")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.username)
                          .WithFiltering(false);
                       cols.Add("Sender_id").WithHeaderText("Sender Id")
                        .WithVisibility(true, true)
                        .WithValueExpression(p => p.sender_id)
                         .WithFiltering(false);
                       cols.Add("password").WithHeaderText("Password")
                        .WithVisibility(true, true)
                        .WithValueExpression(p => p.password)
                         .WithFiltering(false);
                       //cols.Add("IsActive").WithHeaderText("Status")
                       //   .WithVisibility(true, true)
                       //   .WithValueExpression(p => p.IsActive ? "Active" : "Inactive")
                       //   .WithFiltering(true);
                       cols.Add("Createdon")
                              .WithVisibility(visible: false, allowChangeVisibility: false)
                               .WithValueExpression(p => p.CreatedOn.ToString());
                       cols.Add("Action").WithColumnName("Action")
                           .WithVisibility(true, false)
                              .WithSorting(false)
                              .WithHeaderText("Action")
                              .WithHtmlEncoding(false)
                              .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Edit", "Configure", new { id = p.ConfigureId }) + "'>Edit</a> | <a href='" + c.UrlHelper.Action("Details", "Configure", new { id = p.ConfigureId }) + "'>Details</a>")
                              .WithValueTemplate("{Value}");
                   })
                   //.WithAdditionalSetting(MVCGrid.Rendering.BootstrapRenderingEngine.SettingNameTableClass, "notreal") // Example of changing table css class
                   .WithRetrieveDataMethod((context) =>
                   {
                       var options = context.QueryOptions;
                       int totalRecords;
                       int userId = Convert.ToInt32(options.GetPageParameterString("userRole"));
                       var repo = DependencyResolver.Current.GetService<IConfigureServices>();
                       string globalSearch = options.GetAdditionalQueryOptionString("search");
                       string sortColumn = options.GetSortColumnData<string>();
                       var items = repo.GetConfigureData(out totalRecords, options.GetFilterString("ConfigureName"), options.GetLimitOffset(), options.GetLimitRowcount(),
                           sortColumn, options.SortDirection == SortDirection.Dsc,userId);
                       return new QueryResult<ConfigureGridModel>()
                       {
                           Items = items,
                           TotalRecords = totalRecords
                       };
                   })
               );
            #endregion

            #region ClientAdmin
            AddToGridDictonary<ClientAdminGridModel>("ClientAdminGrid");
            MVCGridDefinitionTable.Add("ClientAdminGrid",
                   new MVCGridBuilder<ClientAdminGridModel>(gridDefaults, colDefauls).AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
                   .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                   .WithSorting(sorting: true, defaultSortColumn: "CreatedOn", defaultSortDirection: SortDirection.Dsc)
                   .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                   .WithAdditionalQueryOptionNames("search")
                   .WithFiltering(true)
                   .AddColumns(cols =>
                   {
                       //cols.Add("Id").WithValueExpression((p, c) => c.UrlHelper.Action("detail", "demo", new { id = p.UserId }))
                       //    .WithValueTemplate("<a href='{Value}'>{Model.Id}</a>", false)
                       //    .WithPlainTextValueExpression(p => p.Id.ToString());
                       cols.Add("ClientAdminName").WithHeaderText("Name")
                              .WithVisibility(true, false)
                              .WithValueExpression(p => p.Name)
                              .WithFiltering(true);
                       cols.Add("ClientName").WithHeaderText("Client")
                          .WithVisibility(true, true)
                          .WithValueExpression(p => p.ClientName)
                          .WithFiltering(true);
                       cols.Add("Email")
                           .WithVisibility(true, true)
                           .WithValueExpression(p => p.Email)
                               .WithFiltering(false);
                       cols.Add("ContactNo").WithHeaderText("Contact No")
                          .WithVisibility(true, true)
                          .WithValueExpression(p => p.ContactNo)
                           .WithFiltering(false);
                       cols.Add("IsActive").WithHeaderText("Status")
                          .WithVisibility(true, true)
                          .WithValueExpression(p => p.IsActive ? "Active" : "Inactive")
                          .WithFiltering(true);
                       cols.Add("Createdon")
                              .WithVisibility(visible: false, allowChangeVisibility: false)
                               .WithValueExpression(p => p.CreatedOn.ToString());
                       cols.Add("Action").WithColumnName("Action")
                           .WithVisibility(true, false)
                              .WithSorting(false)
                              .WithHeaderText("Action")
                              .WithHtmlEncoding(false)
                              .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Edit", "BranchAdmin", new { id = p.UserId }) + "'>Edit</a> | <a href='" + c.UrlHelper.Action("Details", "BranchAdmin", new { id = p.UserId }) + "'>Details</a>")
                              .WithValueTemplate("{Value}");
                   })
                   //.WithAdditionalSetting(MVCGrid.Rendering.BootstrapRenderingEngine.SettingNameTableClass, "notreal") // Example of changing table css class
                   .WithRetrieveDataMethod((context) =>
                   {
                       var options = context.QueryOptions;
                       int totalRecords;
                       var repo = DependencyResolver.Current.GetService<IClientAdminService>();
                       string globalSearch = options.GetAdditionalQueryOptionString("search");
                       string sortColumn = options.GetSortColumnData<string>();
                       var items = repo.GetClientAdminData(out totalRecords, options.GetFilterString("ClientAdminName"), options.GetLimitOffset(), options.GetLimitRowcount(),
                           sortColumn, options.SortDirection == SortDirection.Dsc);
                       return new QueryResult<ClientAdminGridModel>()
                       {
                           Items = items,
                           TotalRecords = totalRecords
                       };
                   })
               );
            #endregion

            #region Masterfee
            AddToGridDictonary<MasterFeeGridModel>("MasterFeeGrid");
            MVCGridDefinitionTable.Add("MasterFeeGrid",
            new MVCGridBuilder<MasterFeeGridModel>(gridDefaults, colDefauls).AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
                .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                .WithSorting(sorting: true, defaultSortColumn: "CreatedOn", defaultSortDirection: SortDirection.Dsc)
                .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                .WithAdditionalQueryOptionNames("search")
                .WithFiltering(true)
            .AddColumns(cols =>
            {
                //cols.Add("Id").WithValueExpression((p, c) => c.UrlHelper.Action("detail", "demo", new { id = p.UserId }))
                //    .WithValueTemplate("<a href='{Value}'>{Model.Id}</a>", false)
                //    .WithPlainTextValueExpression(p => p.Id.ToString());
                cols.Add("ClassName").WithHeaderText("Class")
                    .WithVisibility(true, false)
                    .WithValueExpression(p => p.ClassName)
                    .WithFiltering(true);
                cols.Add("SubjectName").WithHeaderText("Subject")
                .WithVisibility(true, true)
                .WithValueExpression(p => p.SubjectName)
                .WithFiltering(true);
                cols.Add("SubjectId").WithHeaderText("Subject Id")
                        .WithVisibility(false, false)
                        .WithValueExpression(p => p.SubjectId.ToString())
                        .WithFiltering(true);
                cols.Add("Year").WithHeaderText("Year")
                .WithVisibility(true, true)
                .WithValueExpression(p => p.Year)
                 .WithFiltering(true);
                cols.Add("Fee").WithHeaderText("Fee")
                .WithVisibility(true, true)
                .WithValueExpression(p => p.Fee.ToString())
                .WithFiltering(true);
                cols.Add("Createdon")
                          .WithVisibility(visible: false, allowChangeVisibility: false)
                           .WithValueExpression(p => p.CreatedOn.ToString());
                cols.Add("Action").WithColumnName("Action")
                .WithVisibility(true, false)
                .WithSorting(false)
                .WithHeaderText("Action")
                .WithHtmlEncoding(false)
                .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Edit", "MasterFee", new { id = p.MasterFeeId }) + "'>Edit</a>")
                .WithValueTemplate("{Value}");
            })
            //.WithAdditionalSetting(MVCGrid.Rendering.BootstrapRenderingEngine.SettingNameTableClass, "notreal") // Example of changing table css class
            .WithRetrieveDataMethod((context) =>
            {
                var options = context.QueryOptions;
                int totalRecords;
                int userId = Convert.ToInt32(options.GetPageParameterString("userRole"));
                var repo = DependencyResolver.Current.GetService<IMasterFeeService>();
                string globalSearch = options.GetAdditionalQueryOptionString("search");
                string sortColumn = options.GetSortColumnData<string>();
                var items = repo.GetMasterFeeDataByClientId(out totalRecords, Convert.ToInt16(options.GetFilterString("ClassName")), Convert.ToInt16(options.GetFilterString("SubjectName")), userId, options.GetLimitOffset(), options.GetLimitRowcount(),
                    sortColumn, options.SortDirection == SortDirection.Dsc);
                return new QueryResult<MasterFeeGridModel>()
                {
                    Items = items,
                    TotalRecords = totalRecords
                };
            })
            );
            #endregion

            #region PDF Category
            AddToGridDictonary<PDFCategoryGridModel>("PDFCategoryGrid");
            MVCGridDefinitionTable.Add("PDFCategoryGrid",
                new MVCGridBuilder<PDFCategoryGridModel>(gridDefaults, colDefauls).AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
                .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                .WithSorting(sorting: true, defaultSortColumn: "Createdon", defaultSortDirection: SortDirection.Dsc)
                .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                .WithAdditionalQueryOptionNames("search")
                .WithFiltering(true)
                .AddColumns(cols =>
                {
                    //cols.Add("Id").WithValueExpression((p, c) => c.UrlHelper.Action("detail", "demo", new { id = p.UserId }))
                    //    .WithValueTemplate("<a href='{Value}'>{Model.Id}</a>", false)
                    //    .WithPlainTextValueExpression(p => p.Id.ToString());
                    cols.Add("Name").WithHeaderText("Name")
                        .WithVisibility(true, false)
                        .WithValueExpression(p => p.Name)
                        .WithFiltering(true);
                    cols.Add("Createdon")
                              .WithVisibility(visible: false, allowChangeVisibility: false)
                               .WithValueExpression(p => p.CreatedOn.ToString());
                    cols.Add("Action").WithColumnName("Action")
                          .WithVisibility(true, false)
                             .WithSorting(false)
                             .WithHeaderText("Action")
                             .WithHtmlEncoding(false)
                             .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Edit", "PDFCategory", new { id = p.PDFCategoryId }) + "'>Edit</a> | <a href='" + c.UrlHelper.Action("Delete", "PDFCategory", new { id = p.PDFCategoryId }) + "'>Delete</a>")
                             .WithValueTemplate("{Value}");
                })
                //.WithAdditionalSetting(MVCGrid.Rendering.BootstrapRenderingEngine.SettingNameTableClass, "notreal") // Example of changing table css class
                .WithRetrieveDataMethod((context) =>
                {
                    var options = context.QueryOptions;
                    int totalRecords;
                    int userId = Convert.ToInt32(options.GetPageParameterString("userRole"));
                    var repo = DependencyResolver.Current.GetService<IPDFCategoryService>();
                    string globalSearch = options.GetAdditionalQueryOptionString("search");
                    string sortColumn = options.GetSortColumnData<string>();
                    var items = repo.GetPDFCategoryData(out totalRecords, options.GetLimitOffset(), options.GetLimitRowcount(),
                        sortColumn, options.SortDirection == SortDirection.Dsc, userId);
                    return new QueryResult<PDFCategoryGridModel>()
                    {
                        Items = items,
                        TotalRecords = totalRecords
                    };
                })
            );
            #endregion

            #region PDF Upload
            AddToGridDictonary<PDFUploadGridModel>("PDFUploadGrid");
            MVCGridDefinitionTable.Add("PDFUploadGrid",
                   new MVCGridBuilder<PDFUploadGridModel>(gridDefaults, colDefauls).AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
                   .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                   .WithSorting(sorting: true, defaultSortColumn: "CreatedOn ", defaultSortDirection: SortDirection.Dsc)
                   .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                   .WithAdditionalQueryOptionNames("search")
                   .WithFiltering(true)
                   .AddColumns(cols =>
                   {
                       //cols.Add("Id").WithValueExpression((p, c) => c.UrlHelper.Action("detail", "demo", new { id = p.UserId }))
                       //    .WithValueTemplate("<a href='{Value}'>{Model.Id}</a>", false)
                       //    .WithPlainTextValueExpression(p => p.Id.ToString());
                       cols.Add("ClassName").WithHeaderText("Class")
                              .WithVisibility(true, false)
                              .WithValueExpression(p => p.ClassName)
                              .WithFiltering(true);
                       cols.Add("PDFCategoryName").WithHeaderText("Category Name")
                          .WithVisibility(true, true)
                          .WithValueExpression(p => p.PDFCategoryName)
                          .WithFiltering(true);
                       cols.Add("Title").WithHeaderText("Title")
                           .WithVisibility(true, true)
                           .WithValueExpression(p => p.Title);
                       cols.Add("FileName").WithHeaderText("File Name")
                          .WithVisibility(true, true)
                          .WithValueExpression(p => p.FileName)
                          .WithFiltering(true);
                       cols.Add("CreatedOn").WithHeaderText("Date")
                          .WithVisibility(true, true)
                          .WithValueExpression(p => p.CreatedOn.ToString("dd/MM/yyyy"))
                          .WithFiltering(true);
                       cols.Add("IsVisible").WithHeaderText("Visible")
                          .WithVisibility(true, true)
                          .WithValueExpression(p => p.IsVisible ? "Visible" : "Invisible")
                          .WithFiltering(true);
                       cols.Add("IsSend").WithHeaderText("Send")
                          .WithVisibility(true, true)
                          .WithValueExpression(p => p.IsSend ? "Send" : "Not Send")
                          .WithFiltering(true);
                       cols.Add().WithColumnName("Download")
                         .WithVisibility(true, true)
                           .WithSorting(false)
                           .WithHeaderText("Download")
                           .WithHtmlEncoding(false)
                           .WithValueExpression((p, c) => c.UrlHelper.Action("DownloadPDF", "PDFUpload", new { id = p.PDFUploadId }))
                           .WithValueTemplate("<a href='{Value}'><span class='glyphicon glyphicon-download-alt'></span></a>");
                       cols.Add("Action").WithColumnName("Action")
                       .WithVisibility(true, false)
                          .WithSorting(false)
                          .WithHeaderText("Action")
                          .WithHtmlEncoding(false)
                          .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Edit", "PDFUpload", new { id = p.PDFUploadId }) + "'>Edit</a> | <a href='" + c.UrlHelper.Action("Delete", "PDFUpload", new { id = p.PDFUploadId }) + "'>Delete</a>")
                          .WithValueTemplate("{Value}");
                   })
                   //.WithAdditionalSetting(MVCGrid.Rendering.BootstrapRenderingEngine.SettingNameTableClass, "notreal") // Example of changing table css class
                   .WithRetrieveDataMethod((context) =>
                   {
                       var options = context.QueryOptions;
                       int totalRecords;
                       var repo = DependencyResolver.Current.GetService<IPDFUploadService>();
                       string globalSearch = options.GetAdditionalQueryOptionString("search");
                       string sortColumn = options.GetSortColumnData<string>();
                       var items = repo.GetPDFUploadData(out totalRecords, options.GetLimitOffset(), options.GetLimitRowcount(),
                           sortColumn, options.SortDirection == SortDirection.Dsc);
                       return new QueryResult<PDFUploadGridModel>()
                       {
                           Items = items,
                           TotalRecords = totalRecords
                       };
                   })
               );
            #endregion

            #region Upload Notes
            MVCGridDefinitionTable.Add("UploadNotesGrid",
                   new MVCGridBuilder<UploadNotesGridModel>(colDefauls)
                  
                   .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                   .WithSorting(sorting: true, defaultSortColumn: "CreatedOn", defaultSortDirection: SortDirection.Dsc)
                   .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                   .WithAdditionalQueryOptionNames("search")
                   .WithFiltering(true)
                   .AddColumns(cols =>
                   {
                       cols.Add("Title").WithHeaderText("Title")
                          .WithVisibility(true, false)
                          .WithValueExpression(p => p.Title)
                          .WithFiltering(true);
                       cols.Add("BoardName").WithHeaderText("Board")
                           .WithVisibility(true, true)
                           .WithValueExpression(p => p.BoardName)
                           .WithFiltering(true);
                       cols.Add("ClassName").WithHeaderText("Class")
                           .WithVisibility(true, true)
                           .WithValueExpression(p => p.ClassName)
                           .WithFiltering(true);
                       cols.Add("SubjectName").WithHeaderText("Subject")
                           .WithVisibility(true, true)
                           .WithValueExpression(p => p.SubjectName)
                           .WithFiltering(true);
                       cols.Add("Date").WithHeaderText("Notes Date")
                          .WithVisibility(true, true)
                          .WithSorting(false)
                          .WithValueExpression(p => p.UploadDate.ToString("dd/MM/yyyy"));
                       cols.Add("CreatedOn").WithHeaderText("Date")
                        .WithVisibility(false, false)
                        .WithSorting(true)
                        .WithValueExpression(p => p.CreatedOn.ToString("dd/MM/yyyy"));
                       cols.Add("FileName").WithHeaderText("File Name")
                          .WithVisibility(true, false)
                          .WithHtmlEncoding(false)
                          .WithValueExpression((p, c) => c.UrlHelper.Action("DownloadPDF", "UploadNotes", new { id = p.UploadNotesId }) + "'>" + p.FileName)
                          .WithValueTemplate("<a  target='_blank' href='{Value}</a>")
                          .WithFiltering(true);
                       cols.Add("IsVisible").WithHeaderText("Visible")
                          .WithVisibility(true, true)
                          .WithValueExpression(p => p.IsVisible ? "Visible" : "Invisible")
                          .WithFiltering(true);
                       cols.Add("LogoPath").WithHeaderText("Logo")
                                        .WithVisibility(true, true)
                                        .WithValueExpression(p => p.LogoName.Replace("~", ".."))
                                        .WithHtmlEncoding(false)
                                        .WithValueTemplate("<img src='{Value}' style='width:40px; height:40px;' />")
                                        .WithSorting(false);
                       cols.Add("Action").WithColumnName("Action")
                                         .WithVisibility(true, false)
                                            .WithSorting(false)
                                            .WithHeaderText("Action")
                                            .WithHtmlEncoding(false)
                                            //.WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Edit", "UploadNotes", new { id = p.UploadNotesId }) + "'>Edit</a> | <a href='" + c.UrlHelper.Action("Delete", "UploadNotes", new { id = p.UploadNotesId }) + "'>Delete</a>")
                                            .WithValueTemplate("{Value}");
                   })
                   .WithRetrieveDataMethod((context) =>
                   {
                       var options = context.QueryOptions;
                       int totalRecords;
                       var repo = DependencyResolver.Current.GetService<IUploadNotesService>();
                       string globalSearch = options.GetAdditionalQueryOptionString("search");
                       string sortColumn = options.GetSortColumnData<string>();
                       var items = repo.GetUploadNotesData(out totalRecords, options.GetLimitOffset(), options.GetLimitRowcount(),
                           sortColumn, options.SortDirection == SortDirection.Dsc);
                       return new QueryResult<UploadNotesGridModel>()
                       {
                           Items = items,
                           TotalRecords = totalRecords
                       };
                   })
               );
            #endregion


            #region Upload Referencebooks
            MVCGridDefinitionTable.Add("UploadReferencebooksGrid",
                   new MVCGridBuilder<UploadReferencebooksGridModel>(colDefauls)
                   .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                   .WithSorting(sorting: true, defaultSortColumn: "CreatedOn", defaultSortDirection: SortDirection.Dsc)
                   .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                   .WithAdditionalQueryOptionNames("search")
                   .WithFiltering(true)
                   .AddColumns(cols =>
                   {
                       cols.Add("Title").WithHeaderText("Title")
                          .WithVisibility(true, false)
                          .WithValueExpression(p => p.Title)
                          .WithFiltering(true);
                       cols.Add("BoardName").WithHeaderText("Board")
                           .WithVisibility(true, true)
                           .WithValueExpression(p => p.BoardName)
                           .WithFiltering(true);
                       cols.Add("ClassName").WithHeaderText("Class")
                           .WithVisibility(true, true)
                           .WithValueExpression(p => p.ClassName)
                           .WithFiltering(true);
                       cols.Add("SubjectName").WithHeaderText("Subject")
                           .WithVisibility(true, true)
                           .WithValueExpression(p => p.SubjectName)
                           .WithFiltering(true);
                       cols.Add("Date").WithHeaderText("Referencebooks Date")
                          .WithVisibility(true, true)
                          .WithSorting(false)
                          .WithValueExpression(p => p.UploadDate.ToString("dd/MM/yyyy"));
                       cols.Add("CreatedOn").WithHeaderText("Date")
                        .WithVisibility(false, false)
                        .WithSorting(true)
                        .WithValueExpression(p => p.CreatedOn.ToString("dd/MM/yyyy"));
                       cols.Add("FileName").WithHeaderText("File Name")
                          .WithVisibility(true, false)
                          .WithHtmlEncoding(false)
                          .WithValueExpression((p, c) => c.UrlHelper.Action("DownloadPDF", "UploadReferencebooks", new { id = p.UploadReferencebooksId }) + "'>" + p.FileName)
                          .WithValueTemplate("<a  target='_blank' href='{Value}</a>")
                          .WithFiltering(true);
                       cols.Add("IsVisible").WithHeaderText("Visible")
                          .WithVisibility(true, true)
                          .WithValueExpression(p => p.IsVisible ? "Visible" : "Invisible")
                          .WithFiltering(true);
                       cols.Add("LogoPath").WithHeaderText("Logo")
                                        .WithVisibility(true, true)
                                        .WithValueExpression(p => p.LogoName.Replace("~", ".."))
                                        .WithHtmlEncoding(false)
                                        .WithValueTemplate("<img src='{Value}' style='width:40px; height:40px;' />")
                                        .WithSorting(false);
                       cols.Add("Action").WithColumnName("Action")
                                         .WithVisibility(true, false)
                                            .WithSorting(false)
                                            .WithHeaderText("Action")
                                            .WithHtmlEncoding(false)
                                            .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Edit", "UploadReferencebooks", new { id = p.UploadReferencebooksId }) + "'>Edit</a> | <a href='" + c.UrlHelper.Action("Delete", "UploadReferencebooks", new { id = p.UploadReferencebooksId }) + "'>Delete</a>")
                                            .WithValueTemplate("{Value}");
                   })
                   .WithRetrieveDataMethod((context) =>
                   {
                       var options = context.QueryOptions;
                       int totalRecords;
                       var repo = DependencyResolver.Current.GetService<IUploadReferencebooksService>();
                       string globalSearch = options.GetAdditionalQueryOptionString("search");
                       string sortColumn = options.GetSortColumnData<string>();
                       var items = repo.GetUploadReferencebooksData(out totalRecords, options.GetLimitOffset(), options.GetLimitRowcount(),
                           sortColumn, options.SortDirection == SortDirection.Dsc);
                       return new QueryResult<UploadReferencebooksGridModel>()
                       {
                           Items = items,
                           TotalRecords = totalRecords
                       };
                   })
               );
            #endregion


            #region Upload Assignments
            MVCGridDefinitionTable.Add("UploadAssignmentsGrid",
                   new MVCGridBuilder<UploadAssignmentsGridModel>(colDefauls)
                   .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                   .WithSorting(sorting: true, defaultSortColumn: "CreatedOn", defaultSortDirection: SortDirection.Dsc)
                   .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                   .WithAdditionalQueryOptionNames("search")
                   .WithFiltering(true)
                   .AddColumns(cols =>
                   {
                       cols.Add("Title").WithHeaderText("Title")
                          .WithVisibility(true, false)
                          .WithValueExpression(p => p.Title)
                          .WithFiltering(true);
                       cols.Add("BoardName").WithHeaderText("Board")
                           .WithVisibility(true, true)
                           .WithValueExpression(p => p.BoardName)
                           .WithFiltering(true);
                       cols.Add("ClassName").WithHeaderText("Class")
                           .WithVisibility(true, true)
                           .WithValueExpression(p => p.ClassName)
                           .WithFiltering(true);
                       cols.Add("SubjectName").WithHeaderText("Subject")
                           .WithVisibility(true, true)
                           .WithValueExpression(p => p.SubjectName)
                           .WithFiltering(true);
                       cols.Add("Date").WithHeaderText("Assignments Date")
                          .WithVisibility(true, true)
                          .WithSorting(false)
                          .WithValueExpression(p => p.UploadDate.ToString("dd/MM/yyyy"));
                       cols.Add("CreatedOn").WithHeaderText("Date")
                        .WithVisibility(false, false)
                        .WithSorting(true)
                        .WithValueExpression(p => p.CreatedOn.ToString("dd/MM/yyyy"));
                       cols.Add("FileName").WithHeaderText("File Name")
                          .WithVisibility(true, false)
                          .WithHtmlEncoding(false)
                          .WithValueExpression((p, c) => c.UrlHelper.Action("DownloadPDF", "UploadAssignments", new { id = p.UploadAssignmentsId }) + "'>" + p.FileName)
                          .WithValueTemplate("<a  target='_blank' href='{Value}</a>")
                          .WithFiltering(true);
                       cols.Add("IsVisible").WithHeaderText("Visible")
                          .WithVisibility(true, true)
                          .WithValueExpression(p => p.IsVisible ? "Visible" : "Invisible")
                          .WithFiltering(true);
                       cols.Add("LogoPath").WithHeaderText("Logo")
                                        .WithVisibility(true, true)
                                        .WithValueExpression(p => p.LogoName.Replace("~", ".."))
                                        .WithHtmlEncoding(false)
                                        .WithValueTemplate("<img src='{Value}' style='width:40px; height:40px;' />")
                                        .WithSorting(false);
                       cols.Add("Action").WithColumnName("Action")
                                         .WithVisibility(true, false)
                                            .WithSorting(false)
                                            .WithHeaderText("Action")
                                            .WithHtmlEncoding(false)
                                            .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Edit", "UploadAssignments", new { id = p.UploadAssignmentsId }) + "'>Edit</a> | <a href='" + c.UrlHelper.Action("Delete", "UploadAssignments", new { id = p.UploadAssignmentsId }) + "'>Delete</a>")
                                            .WithValueTemplate("{Value}");
                   })
                   .WithRetrieveDataMethod((context) =>
                   {
                       var options = context.QueryOptions;
                       int totalRecords;
                       var repo = DependencyResolver.Current.GetService<IUploadAssignmentsService>();
                       string globalSearch = options.GetAdditionalQueryOptionString("search");
                       string sortColumn = options.GetSortColumnData<string>();
                       var items = repo.GetUploadAssignmentsData(out totalRecords, options.GetLimitOffset(), options.GetLimitRowcount(),
                           sortColumn, options.SortDirection == SortDirection.Dsc);
                       return new QueryResult<UploadAssignmentsGridModel>()
                       {
                           Items = items,
                           TotalRecords = totalRecords
                       };
                   })
               );
            #endregion

            #region Upload Textbooks
            MVCGridDefinitionTable.Add("UploadTextbooksGrid",
                   new MVCGridBuilder<UploadTextbooksGridModel>(colDefauls)
                   .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                   .WithSorting(sorting: true, defaultSortColumn: "CreatedOn", defaultSortDirection: SortDirection.Dsc)
                   .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                   .WithAdditionalQueryOptionNames("search")
                   .WithFiltering(true)
                   .AddColumns(cols =>
                   {
                       cols.Add("Title").WithHeaderText("Title")
                          .WithVisibility(true, false)
                          .WithValueExpression(p => p.Title)
                          .WithFiltering(true);
                       cols.Add("BoardName").WithHeaderText("Board")
                           .WithVisibility(true, true)
                           .WithValueExpression(p => p.BoardName)
                           .WithFiltering(true);
                       cols.Add("ClassName").WithHeaderText("Class")
                           .WithVisibility(true, true)
                           .WithValueExpression(p => p.ClassName)
                           .WithFiltering(true);
                       cols.Add("SubjectName").WithHeaderText("Subject")
                           .WithVisibility(true, true)
                           .WithValueExpression(p => p.SubjectName)
                           .WithFiltering(true);
                       cols.Add("Date").WithHeaderText("Textbooks Date")
                          .WithVisibility(true, true)
                          .WithSorting(false)
                          .WithValueExpression(p => p.UploadDate.ToString("dd/MM/yyyy"));
                       cols.Add("CreatedOn").WithHeaderText("Date")
                        .WithVisibility(false, false)
                        .WithSorting(true)
                        .WithValueExpression(p => p.CreatedOn.ToString("dd/MM/yyyy"));
                       cols.Add("FileName").WithHeaderText("File Name")
                          .WithVisibility(true, false)
                          .WithHtmlEncoding(false)
                          .WithValueExpression((p, c) => c.UrlHelper.Action("DownloadPDF", "UploadTextbooks", new { id = p.UploadTextbooksId }) + "'>" + p.FileName)
                          .WithValueTemplate("<a  target='_blank' href='{Value}</a>")
                          .WithFiltering(true);
                       cols.Add("IsVisible").WithHeaderText("Visible")
                          .WithVisibility(true, true)
                          .WithValueExpression(p => p.IsVisible ? "Visible" : "Invisible")
                          .WithFiltering(true);
                       cols.Add("LogoPath").WithHeaderText("Logo")
                                        .WithVisibility(true, true)
                                        .WithValueExpression(p => p.LogoName.Replace("~", ".."))
                                        .WithHtmlEncoding(false)
                                        .WithValueTemplate("<img src='{Value}' style='width:40px; height:40px;' />")
                                        .WithSorting(false);
                       cols.Add("Action").WithColumnName("Action")
                                         .WithVisibility(true, false)
                                            .WithSorting(false)
                                            .WithHeaderText("Action")
                                            .WithHtmlEncoding(false)
                                            .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Edit", "UploadTextbooks", new { id = p.UploadTextbooksId }) + "'>Edit</a> | <a href='" + c.UrlHelper.Action("Delete", "UploadTextbooks", new { id = p.UploadTextbooksId }) + "'>Delete</a>")
                                            .WithValueTemplate("{Value}");
                   })
                   .WithRetrieveDataMethod((context) =>
                   {
                       var options = context.QueryOptions;
                       int totalRecords;
                       var repo = DependencyResolver.Current.GetService<IUploadTextbooksService>();
                       string globalSearch = options.GetAdditionalQueryOptionString("search");
                       string sortColumn = options.GetSortColumnData<string>();
                       var items = repo.GetUploadTextbooksData(out totalRecords, options.GetLimitOffset(), options.GetLimitRowcount(),
                           sortColumn, options.SortDirection == SortDirection.Dsc);
                       return new QueryResult<UploadTextbooksGridModel>()
                       {
                           Items = items,
                           TotalRecords = totalRecords
                       };
                   })
               );
            #endregion



            #region Upload Inbuiltquestionbank
            MVCGridDefinitionTable.Add("UploadInbuiltquestionbankGrid",
                   new MVCGridBuilder<UploadInbuiltquestionbankGridModel>(colDefauls)
                   .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                   .WithSorting(sorting: true, defaultSortColumn: "CreatedOn", defaultSortDirection: SortDirection.Dsc)
                   .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                   .WithAdditionalQueryOptionNames("search")
                   .WithFiltering(true)
                   .AddColumns(cols =>
                   {
                       cols.Add("Title").WithHeaderText("Title")
                          .WithVisibility(true, false)
                          .WithValueExpression(p => p.Title)
                          .WithFiltering(true);
                       cols.Add("BoardName").WithHeaderText("Board")
                           .WithVisibility(true, true)
                           .WithValueExpression(p => p.BoardName)
                           .WithFiltering(true);
                       cols.Add("ClassName").WithHeaderText("Class")
                           .WithVisibility(true, true)
                           .WithValueExpression(p => p.ClassName)
                           .WithFiltering(true);
                       cols.Add("SubjectName").WithHeaderText("Subject")
                           .WithVisibility(true, true)
                           .WithValueExpression(p => p.SubjectName)
                           .WithFiltering(true);
                       cols.Add("Date").WithHeaderText("Inbuiltquestionbank Date")
                          .WithVisibility(true, true)
                          .WithSorting(false)
                          .WithValueExpression(p => p.UploadDate.ToString("dd/MM/yyyy"));
                       cols.Add("CreatedOn").WithHeaderText("Date")
                        .WithVisibility(false, false)
                        .WithSorting(true)
                        .WithValueExpression(p => p.CreatedOn.ToString("dd/MM/yyyy"));
                       cols.Add("FileName").WithHeaderText("File Name")
                          .WithVisibility(true, false)
                          .WithHtmlEncoding(false)
                          .WithValueExpression((p, c) => c.UrlHelper.Action("DownloadPDF", "UploadInbuiltquestionbank", new { id = p.UploadInbuiltquestionbankId }) + "'>" + p.FileName)
                          .WithValueTemplate("<a  target='_blank' href='{Value}</a>")
                          .WithFiltering(true);
                       cols.Add("IsVisible").WithHeaderText("Visible")
                          .WithVisibility(true, true)
                          .WithValueExpression(p => p.IsVisible ? "Visible" : "Invisible")
                          .WithFiltering(true);
                       cols.Add("LogoPath").WithHeaderText("Logo")
                                        .WithVisibility(true, true)
                                        .WithValueExpression(p => p.LogoName.Replace("~", ".."))
                                        .WithHtmlEncoding(false)
                                        .WithValueTemplate("<img src='{Value}' style='width:40px; height:40px;' />")
                                        .WithSorting(false);
                       cols.Add("Action").WithColumnName("Action")
                                         .WithVisibility(true, false)
                                            .WithSorting(false)
                                            .WithHeaderText("Action")
                                            .WithHtmlEncoding(false)
                                            .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Edit", "UploadInbuiltquestionbank", new { id = p.UploadInbuiltquestionbankId }) + "'>Edit</a> | <a href='" + c.UrlHelper.Action("Delete", "UploadInbuiltquestionbank", new { id = p.UploadInbuiltquestionbankId }) + "'>Delete</a>")
                                            .WithValueTemplate("{Value}");
                   })
                   .WithRetrieveDataMethod((context) =>
                   {
                       var options = context.QueryOptions;
                       int totalRecords;
                       var repo = DependencyResolver.Current.GetService<IUploadInbuiltquestionbankService>();
                       string globalSearch = options.GetAdditionalQueryOptionString("search");
                       string sortColumn = options.GetSortColumnData<string>();
                       var items = repo.GetUploadInbuiltquestionbankData(out totalRecords, options.GetLimitOffset(), options.GetLimitRowcount(),
                           sortColumn, options.SortDirection == SortDirection.Dsc);
                       return new QueryResult<UploadInbuiltquestionbankGridModel>()
                       {
                           Items = items,
                           TotalRecords = totalRecords
                       };
                   })
               );
            #endregion



            #region Upload Practicepapers
            MVCGridDefinitionTable.Add("UploadPracticepapersGrid",
                   new MVCGridBuilder<UploadPracticepapersGridModel>(colDefauls)
                   .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                   .WithSorting(sorting: true, defaultSortColumn: "CreatedOn", defaultSortDirection: SortDirection.Dsc)
                   .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                   .WithAdditionalQueryOptionNames("search")
                   .WithFiltering(true)
                   .AddColumns(cols =>
                   {
                       cols.Add("Title").WithHeaderText("Title")
                          .WithVisibility(true, false)
                          .WithValueExpression(p => p.Title)
                          .WithFiltering(true);
                       cols.Add("BoardName").WithHeaderText("Board")
                           .WithVisibility(true, true)
                           .WithValueExpression(p => p.BoardName)
                           .WithFiltering(true);
                       cols.Add("ClassName").WithHeaderText("Class")
                           .WithVisibility(true, true)
                           .WithValueExpression(p => p.ClassName)
                           .WithFiltering(true);
                       cols.Add("SubjectName").WithHeaderText("Subject")
                           .WithVisibility(true, true)
                           .WithValueExpression(p => p.SubjectName)
                           .WithFiltering(true);
                       cols.Add("Date").WithHeaderText("Practicepapers Date")
                          .WithVisibility(true, true)
                          .WithSorting(false)
                          .WithValueExpression(p => p.UploadDate.ToString("dd/MM/yyyy"));
                       cols.Add("CreatedOn").WithHeaderText("Date")
                        .WithVisibility(false, false)
                        .WithSorting(true)
                        .WithValueExpression(p => p.CreatedOn.ToString("dd/MM/yyyy"));
                       cols.Add("FileName").WithHeaderText("File Name")
                          .WithVisibility(true, false)
                          .WithHtmlEncoding(false)
                          .WithValueExpression((p, c) => c.UrlHelper.Action("DownloadPDF", "UploadPracticepapers", new { id = p.UploadPracticepapersId }) + "'>" + p.FileName)
                          .WithValueTemplate("<a  target='_blank' href='{Value}</a>")
                          .WithFiltering(true);
                       cols.Add("IsVisible").WithHeaderText("Visible")
                          .WithVisibility(true, true)
                          .WithValueExpression(p => p.IsVisible ? "Visible" : "Invisible")
                          .WithFiltering(true);
                       cols.Add("LogoPath").WithHeaderText("Logo")
                                        .WithVisibility(true, true)
                                        .WithValueExpression(p => p.LogoName.Replace("~", ".."))
                                        .WithHtmlEncoding(false)
                                        .WithValueTemplate("<img src='{Value}' style='width:40px; height:40px;' />")
                                        .WithSorting(false);
                       cols.Add("Action").WithColumnName("Action")
                                         .WithVisibility(true, false)
                                            .WithSorting(false)
                                            .WithHeaderText("Action")
                                            .WithHtmlEncoding(false)
                                            .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Edit", "UploadPracticepapers", new { id = p.UploadPracticepapersId }) + "'>Edit</a> | <a href='" + c.UrlHelper.Action("Delete", "UploadPracticepapers", new { id = p.UploadPracticepapersId }) + "'>Delete</a>")
                                            .WithValueTemplate("{Value}");
                   })
                   .WithRetrieveDataMethod((context) =>
                   {
                       var options = context.QueryOptions;
                       int totalRecords;
                       var repo = DependencyResolver.Current.GetService<IUploadPracticepapersService>();
                       string globalSearch = options.GetAdditionalQueryOptionString("search");
                       string sortColumn = options.GetSortColumnData<string>();
                       var items = repo.GetUploadPracticepapersData(out totalRecords, options.GetLimitOffset(), options.GetLimitRowcount(),
                           sortColumn, options.SortDirection == SortDirection.Dsc);
                       return new QueryResult<UploadPracticepapersGridModel>()
                       {
                           Items = items,
                           TotalRecords = totalRecords
                       };
                   })
               );
            #endregion



            #region Upload Questionpapers
            MVCGridDefinitionTable.Add("UploadQuestionpapersGrid",
                   new MVCGridBuilder<UploadQuestionpapersGridModel>(colDefauls)
                   .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                   .WithSorting(sorting: true, defaultSortColumn: "CreatedOn", defaultSortDirection: SortDirection.Dsc)
                   .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                   .WithAdditionalQueryOptionNames("search")
                   .WithFiltering(true)
                   .AddColumns(cols =>
                   {
                       cols.Add("Title").WithHeaderText("Title")
                          .WithVisibility(true, false)
                          .WithValueExpression(p => p.Title)
                          .WithFiltering(true);
                       cols.Add("BoardName").WithHeaderText("Board")
                           .WithVisibility(true, true)
                           .WithValueExpression(p => p.BoardName)
                           .WithFiltering(true);
                       cols.Add("ClassName").WithHeaderText("Class")
                           .WithVisibility(true, true)
                           .WithValueExpression(p => p.ClassName)
                           .WithFiltering(true);
                       cols.Add("SubjectName").WithHeaderText("Subject")
                           .WithVisibility(true, true)
                           .WithValueExpression(p => p.SubjectName)
                           .WithFiltering(true);
                       cols.Add("Date").WithHeaderText("Questionpapers Date")
                          .WithVisibility(true, true)
                          .WithSorting(false)
                          .WithValueExpression(p => p.UploadDate.ToString("dd/MM/yyyy"));
                       cols.Add("CreatedOn").WithHeaderText("Date")
                        .WithVisibility(false, false)
                        .WithSorting(true)
                        .WithValueExpression(p => p.CreatedOn.ToString("dd/MM/yyyy"));
                       cols.Add("FileName").WithHeaderText("File Name")
                          .WithVisibility(true, false)
                          .WithHtmlEncoding(false)
                          .WithValueExpression((p, c) => c.UrlHelper.Action("DownloadPDF", "UploadQuestionpapers", new { id = p.UploadQuestionpapersId }) + "'>" + p.FileName)
                          .WithValueTemplate("<a  target='_blank' href='{Value}</a>")
                          .WithFiltering(true);
                       cols.Add("IsVisible").WithHeaderText("Visible")
                          .WithVisibility(true, true)
                          .WithValueExpression(p => p.IsVisible ? "Visible" : "Invisible")
                          .WithFiltering(true);
                       cols.Add("LogoPath").WithHeaderText("Logo")
                                        .WithVisibility(true, true)
                                        .WithValueExpression(p => p.LogoName.Replace("~", ".."))
                                        .WithHtmlEncoding(false)
                                        .WithValueTemplate("<img src='{Value}' style='width:40px; height:40px;' />")
                                        .WithSorting(false);
                       cols.Add("Action").WithColumnName("Action")
                                         .WithVisibility(true, false)
                                            .WithSorting(false)
                                            .WithHeaderText("Action")
                                            .WithHtmlEncoding(false)
                                            .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Edit", "UploadQuestionpapers", new { id = p.UploadQuestionpapersId }) + "'>Edit</a> | <a href='" + c.UrlHelper.Action("Delete", "UploadQuestionpapers", new { id = p.UploadQuestionpapersId }) + "'>Delete</a>")
                                            .WithValueTemplate("{Value}");
                   })
                   .WithRetrieveDataMethod((context) =>
                   {
                       var options = context.QueryOptions;
                       int totalRecords;
                       var repo = DependencyResolver.Current.GetService<IUploadQuestionpapersService>();
                       string globalSearch = options.GetAdditionalQueryOptionString("search");
                       string sortColumn = options.GetSortColumnData<string>();
                       var items = repo.GetUploadQuestionpapersData(out totalRecords, options.GetLimitOffset(), options.GetLimitRowcount(),
                           sortColumn, options.SortDirection == SortDirection.Dsc);
                       return new QueryResult<UploadQuestionpapersGridModel>()
                       {
                           Items = items,
                           TotalRecords = totalRecords
                       };
                   })
               );
            #endregion



            #region Machine
            AddToGridDictonary<MachineGridModel>("MachineGrid");
            MVCGridDefinitionTable.Add("MachineGrid",
                 new MVCGridBuilder<MachineGridModel>(gridDefaults, colDefauls).AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
                 .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                 .WithSorting(sorting: true, defaultSortColumn: "Createdon", defaultSortDirection: SortDirection.Dsc)
                 .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                 .WithAdditionalQueryOptionNames("search")
                 .WithFiltering(true)
                 .WithPageParameterNames("userRole")
                 .AddColumns(cols =>
                 {
                     //cols.Add("Id").WithValueExpression((p, c) => c.UrlHelper.Action("detail", "demo", new { id = p.UserId }))
                     //    .WithValueTemplate("<a href='{Value}'>{Model.Id}</a>", false)
                     //    .WithPlainTextValueExpression(p => p.Id.ToString());
                     cols.Add("Name").WithHeaderText("Name")
                          .WithVisibility(true, false)
                          .WithValueExpression(p => p.Name)
                          .WithFiltering(true);
                     cols.Add("BranchName").WithHeaderText("Branch Name")
                        .WithVisibility(true, true)
                        .WithValueExpression(p => p.BranchName)
                        .WithFiltering(true);
                     cols.Add("SerialNumber").WithHeaderText("Serial Number")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.SerialNumber)
                         .WithFiltering(true);
                     cols.Add("Createdon")
                              .WithVisibility(visible: false, allowChangeVisibility: false)
                               .WithValueExpression(p => p.CreatedOn.ToString());
                     cols.Add("Action").WithColumnName("Action")
                       .WithVisibility(true, false)
                          .WithSorting(false)
                          .WithHeaderText("Action")
                          .WithHtmlEncoding(false)
                          .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Edit", "Machine", new { id = p.MachineId }) + "'>Edit</a> | <a href='" + c.UrlHelper.Action("Delete", "Machine", new { id = p.MachineId }) + "'>Delete</a>")
                          .WithValueTemplate("{Value}");
                 })
                 //.WithAdditionalSetting(MVCGrid.Rendering.BootstrapRenderingEngine.SettingNameTableClass, "notreal") // Example of changing table css class
                 .WithRetrieveDataMethod((context) =>
                 {
                     var options = context.QueryOptions;
                     int totalRecords;
                     int userId = Convert.ToInt16(options.GetPageParameterString("userRole"));
                     var repo = DependencyResolver.Current.GetService<IMachineService>();
                     string globalSearch = options.GetAdditionalQueryOptionString("search");
                     string sortColumn = options.GetSortColumnData<string>();
                     var items = repo.GetMachineData(out totalRecords, userId, options.GetLimitOffset(), options.GetLimitRowcount(),
                         sortColumn, options.SortDirection == SortDirection.Dsc);
                     return new QueryResult<MachineGridModel>()
                     {
                         Items = items,
                         TotalRecords = totalRecords
                     };
                 })
             );
            #endregion

            #region Attendance
            AddToGridDictonary<AttendanceGridModel>("AttendanceGrid");
            MVCGridDefinitionTable.Add("AttendanceGrid",
                 new MVCGridBuilder<AttendanceGridModel>(gridDefaults, colDefauls).AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
                 .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                 .WithSorting(sorting: true, defaultSortColumn: "CreatedOn", defaultSortDirection: SortDirection.Dsc)
                 .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                 .WithAdditionalQueryOptionNames("search")
                 .WithFiltering(true)
                  .WithPageParameterNames("userRole")
                 .AddColumns(cols =>
                 {
                     //cols.Add("Id").WithValueExpression((p, c) => c.UrlHelper.Action("detail", "demo", new { id = p.UserId }))
                     //    .WithValueTemplate("<a href='{Value}'>{Model.Id}</a>", false)
                     //    .WithPlainTextValueExpression(p => p.Id.ToString());
                     //cols.Add()
                     //       .WithHtmlEncoding(false)
                     //       .WithValueExpression((p) => "Html.CheckBox('checked', false)")
                     //       .WithValueTemplate("{Value}");
                     cols.Add("Select")
                          .WithHtmlEncoding(false)
                          .WithSorting(false)
                          .WithFiltering(false)
                          .WithHeaderText("Select")
                          .WithValueExpression((p, c) => c.UrlHelper.Action("", "", new { area = "General", id = p.AttendanceId }))
                          .WithValueTemplate("<input type='checkbox' class='select' value='{Model.AttendanceId}' onclick='selectedValue()'>")
                          .WithPlainTextValueExpression((p, c) => "");
                     cols.Add("BranchName").WithHeaderText("Branch")
                          .WithVisibility(true, false)
                          .WithValueExpression(p => p.BranchName)
                          .WithFiltering(true);
                     cols.Add("ClassName").WithHeaderText("Class")
                        .WithVisibility(true, true)
                        .WithValueExpression(p => p.ClassName)
                        .WithFiltering(true);
                     //cols.Add("SubjectName").WithHeaderText("Subject")
                     //    .WithVisibility(true, true)
                     //    .WithValueExpression(p => p.SubjectName)
                     //    .WithFiltering(true);
                     //cols.Add("SubjectId").WithHeaderText("Subject Id")
                     //  .WithVisibility(false, false)
                     //  .WithValueExpression(p => p.SubjectId.ToString())
                     //  .WithFiltering(true);
                     cols.Add("BatchName").WithHeaderText("Batch")
                        .WithVisibility(true, true)
                        .WithValueExpression(p => p.BatchName)
                        .WithFiltering(true);
                     cols.Add("Date").WithHeaderText("Date")
                        .WithVisibility(true, true)
                        .WithValueExpression(p => Convert.ToDateTime(p.Date).ToString("dd/MM/yyyy"))
                        .WithFiltering(true);
                     cols.Add("TeacherName").WithHeaderText("Teacher")
                        .WithVisibility(true, true)
                        .WithValueExpression(p => p.TeacherName)
                        .WithFiltering(true);
                     cols.Add("Activity").WithHeaderText("Activity")
                        .WithVisibility(true, true)
                        .WithValueExpression(p => p.Activity)
                        .WithFiltering(true);
                     cols.Add("IsManual").WithHeaderText("Status")
                        .WithVisibility(true, true)
                        .WithValueExpression(p => p.IsManual ? "Manual" : "Automate")
                        .WithFiltering(true);
                     cols.Add("IsSend").WithHeaderText("Send Status")
                        .WithVisibility(true, true)
                        .WithValueExpression(p => p.IsSend ? "Send" : "Not Send")
                        .WithFiltering(true);
                     cols.Add("Createdon")
                              .WithVisibility(visible: false, allowChangeVisibility: false)
                               .WithValueExpression(p => p.CreatedOn.ToString());
                     cols.Add("Action").WithColumnName("Action")
                        .WithVisibility(true, false)
                        .WithSorting(false)
                        .WithHeaderText("Action")
                        .WithHtmlEncoding(false)
                        .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Edit", "Attendance", new { id = p.AttendanceId }) + "'>Edit</a> | <a href='" + c.UrlHelper.Action("GeneratePdf", "Attendance", new { id = p.AttendanceId }) + "'>Generate PDF</a>")
                        .WithValueTemplate("{Value}");
                 })
                 //.WithAdditionalSetting(MVCGrid.Rendering.BootstrapRenderingEngine.SettingNameTableClass, "notreal") // Example of changing table css class
                 .WithRetrieveDataMethod((context) =>
                 {
                     var options = context.QueryOptions;
                     int totalRecords;
                     int userId = Convert.ToInt16(options.GetPageParameterString("userRole"));
                     var repo = DependencyResolver.Current.GetService<IAttendanceService>();
                     string globalSearch = options.GetAdditionalQueryOptionString("search");
                     string sortColumn = options.GetSortColumnData<string>();
                     var items = repo.GetAttendanceData(out totalRecords, Convert.ToInt16(options.GetFilterString("ClassName")), Convert.ToInt16(options.GetFilterString("SubjectName")), Convert.ToDateTime(options.GetFilterString("Date")), userId, options.GetLimitOffset(), options.GetLimitRowcount(),
                         sortColumn, options.SortDirection == SortDirection.Dsc);
                     return new QueryResult<AttendanceGridModel>()
                     {
                         Items = items,
                         TotalRecords = totalRecords
                     };
                 })
             );
            #endregion

            #region Announcement
            AddToGridDictonary<AnnouncementGridModel>("AnnouncementGrid");
            MVCGridDefinitionTable.Add("AnnouncementGrid",
                   new MVCGridBuilder<AnnouncementGridModel>(gridDefaults, colDefauls).AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
                   .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                   .WithSorting(sorting: true, defaultSortColumn: "CreatedOn", defaultSortDirection: SortDirection.Dsc)
                   .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                   .WithAdditionalQueryOptionNames("search")
                   .WithFiltering(true)
                   .AddColumns(cols =>
                   {
                       //cols.Add("Id").WithValueExpression((p, c) => c.UrlHelper.Action("detail", "demo", new { id = p.UserId }))
                       //    .WithValueTemplate("<a href='{Value}'>{Model.Id}</a>", false)
                       //    .WithPlainTextValueExpression(p => p.Id.ToString());
                       cols.Add("AnnouncementDetails").WithHeaderText("Announcement Details")
                              .WithVisibility(true, false)
                              .WithValueExpression(p => p.AnnouncementDetails)
                              .WithFiltering(true);
                       cols.Add("Url").WithHeaderText("Url")
                          .WithVisibility(true, true)
                          .WithValueExpression(p => p.Url)
                          .WithFiltering(true);
                       cols.Add("IsVisible").WithHeaderText("Visible")
                           .WithVisibility(true, true)
                           .WithValueExpression(p => p.IsVisible ? "Visible" : "Invisible")
                           .WithFiltering(true);
                       cols.Add("CreatedOn").WithHeaderText("Date")
                          .WithVisibility(true, true)
                          .WithValueExpression(p => p.CreatedOn.ToString("dd/MM/yyy"))
                          .WithFiltering(true);
                       cols.Add("Action").WithColumnName("Action")
                     .WithVisibility(true, false)
                        .WithSorting(false)
                        .WithHeaderText("Action")
                        .WithHtmlEncoding(false)
                        .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Edit", "Announcement", new { id = p.AnnouncementId }) + "'>Edit</a> | <a href='" + c.UrlHelper.Action("Delete", "Announcement", new { id = p.AnnouncementId }) + "'>Delete</a>")
                        .WithValueTemplate("{Value}");
                   })
                   //.WithAdditionalSetting(MVCGrid.Rendering.BootstrapRenderingEngine.SettingNameTableClass, "notreal") // Example of changing table css class
                   .WithRetrieveDataMethod((context) =>
                   {
                       var options = context.QueryOptions;
                       int totalRecords;
                       var repo = DependencyResolver.Current.GetService<IAnnouncementService>();
                       string globalSearch = options.GetAdditionalQueryOptionString("search");
                       string sortColumn = options.GetSortColumnData<string>();
                       var items = repo.GetAnnouncementData(out totalRecords, options.GetLimitOffset(), options.GetLimitRowcount(),
                           sortColumn, options.SortDirection == SortDirection.Dsc);
                       return new QueryResult<AnnouncementGridModel>()
                       {
                           Items = items,
                           TotalRecords = totalRecords
                       };
                   })
               );
            #endregion

            #region TestPaper
            AddToGridDictonary<TestPaperGridModel>("TestPaperGrid");
            MVCGridDefinitionTable.Add("TestPaperGrid",
                  new MVCGridBuilder<TestPaperGridModel>(gridDefaults, colDefauls).AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
                  .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                  .WithSorting(sorting: true, defaultSortColumn: "CreatedOn", defaultSortDirection: SortDirection.Dsc)
                  .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                  .WithAdditionalQueryOptionNames("search")
                  .WithFiltering(true)
                  .AddColumns(cols =>
                  {
                      //cols.Add("Id").WithValueExpression((p, c) => c.UrlHelper.Action("detail", "demo", new { id = p.UserId }))
                      //    .WithValueTemplate("<a href='{Value}'>{Model.Id}</a>", false)
                      //    .WithPlainTextValueExpression(p => p.Id.ToString());
                      cols.Add("Select")
                          .WithHtmlEncoding(false)
                          .WithSorting(false)
                          .WithFiltering(false)
                          .WithHeaderText("Select")
                          .WithValueExpression((p, c) => c.UrlHelper.Action("", "", new { area = "General", id = p.TestPaperId }))
                          .WithValueTemplate("<input type='checkbox' class='select' value='{Model.TestPaperId}'>")
                          .WithPlainTextValueExpression((p, c) => "");
                      cols.Add("Title").WithHeaderText("Title")
                            .WithVisibility(true, false)
                            .WithValueExpression(p => p.Title)
                            .WithFiltering(true);
                      cols.Add("ClassName").WithHeaderText("Class")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.ClassName)
                         .WithFiltering(true);
                      cols.Add("TestTaken").WithHeaderText("Test Taken")
                          .WithVisibility(true, true)
                          .WithValueExpression(p => p.TestTaken ? "Completed" : "Pending")
                          .WithFiltering(true);
                      cols.Add("CreatedOn").WithHeaderText("Date")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.CreatedOn.ToString("dd/MM/yyyy"))
                          .WithFiltering(true);
                      cols.Add("TestType").WithHeaderText("Test Type")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.TestType.ToString())
                         .WithFiltering(true);
                      cols.Add("QuestionCount").WithHeaderText("Question Count")
                        .WithVisibility(true, true)
                        .WithValueExpression(p => p.QuestionCount.ToString())
                        .WithFiltering(true);
                      cols.Add("SubjectName").WithHeaderText("Subject Name")
                        .WithVisibility(true, true)
                        .WithValueExpression(p => p.SubjectName)
                        .WithFiltering(true);
                      cols.Add("Action").WithColumnName("Action")
                  .WithVisibility(true, false)
                     .WithSorting(false)
                     .WithHeaderText("Action")
                     .WithHtmlEncoding(false)
                     .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Create", "Paper", new { id = p.TestPaperId }) + "'>Edit</a> | <a href='" + c.UrlHelper.Action("Delete", "Paper", new { id = p.TestPaperId }) + "'>Delete</a>")
                     .WithValueTemplate("{Value}");
                  })
                  //.WithAdditionalSetting(MVCGrid.Rendering.BootstrapRenderingEngine.SettingNameTableClass, "notreal") // Example of changing table css class
                  .WithRetrieveDataMethod((context) =>
                  {
                      var options = context.QueryOptions;
                      int totalRecords;
                      var repo = DependencyResolver.Current.GetService<ITestPaperService>();
                      string globalSearch = options.GetAdditionalQueryOptionString("search");
                      string sortColumn = options.GetSortColumnData<string>();
                      var items = repo.GetTestPaperData(out totalRecords, options.GetLimitOffset(), options.GetLimitRowcount(),
                          sortColumn, options.SortDirection == SortDirection.Dsc);
                      return new QueryResult<TestPaperGridModel>()
                      {
                          Items = items,
                          TotalRecords = totalRecords
                      };
                  })
              );
            #endregion

            #region Notification
            AddToGridDictonary<BranchGridModel>("NotificationGrid");
            MVCGridDefinitionTable.Add("NotificationGrid",
                 new MVCGridBuilder<NotificationGridModel>(gridDefaults, colDefauls).AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
                 .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                 .WithSorting(sorting: true, defaultSortColumn: "CreatedOn", defaultSortDirection: SortDirection.Dsc)
                 .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                 .WithAdditionalQueryOptionNames("search")
                 .WithFiltering(true)
                 .AddColumns(cols =>
                 {
                     //cols.Add("Id").WithValueExpression((p, c) => c.UrlHelper.Action("detail", "demo", new { id = p.UserId }))
                     //    .WithValueTemplate("<a href='{Value}'>{Model.Id}</a>", false)
                     //    .WithPlainTextValueExpression(p => p.Id.ToString());
                     cols.Add("NotificationMessage").WithHeaderText("Notification Message")
                          .WithVisibility(true, false)
                          .WithValueExpression(p => p.NotificationMessage)
                          .WithFiltering(true);
                     cols.Add("CreatedOn").WithHeaderText("Created Date")
                        .WithVisibility(true, true)
                        .WithValueExpression(p => p.CreatedOn.ToString("dd/MM/yyyy"));
                     cols.Add("ClassName").WithHeaderText("All User")
                        .WithVisibility(true, true)
                        .WithValueExpression(p => p.AllUser ? "All User" : "Not All User")
                        .WithFiltering(true);
                     cols.Add("BranchAdminCount").WithHeaderText("Branch Admin Count")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.BranchAdminCount.ToString())
                         .WithFiltering(true);
                     cols.Add("TeacherCount").WithHeaderText("Teacher Count")
                        .WithVisibility(true, true)
                        .WithValueExpression(p => p.TeacherCount.ToString())
                        .WithFiltering(true);
                     cols.Add("StudentCount").WithHeaderText("Student Count")
                        .WithVisibility(true, true)
                        .WithValueExpression(p => p.StudentCount.ToString())
                        .WithFiltering(true);
                     cols.Add("ParentCount").WithHeaderText("Parent Count")
                        .WithVisibility(true, true)
                        .WithValueExpression(p => p.ParentCount.ToString())
                        .WithFiltering(true);
                     cols.Add("Media").WithHeaderText("Media")
                        .WithVisibility(true, true)
                        .WithValueExpression(p => p.Media.ToString())
                        .WithFiltering(true);
                     cols.Add("Action").WithColumnName("Action")
                 .WithVisibility(true, true)
                    .WithSorting(false)
                    .WithHeaderText("Action")
                    .WithHtmlEncoding(false)
                    .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Details", "Notification", new { id = p.NotificationId }) + "'>Details</a>")
                    .WithValueTemplate("{Value}");
                 })
                 //.WithAdditionalSetting(MVCGrid.Rendering.BootstrapRenderingEngine.SettingNameTableClass, "notreal") // Example of changing table css class
                 .WithRetrieveDataMethod((context) =>
                 {
                     var options = context.QueryOptions;
                     int totalRecords;
                     int userId = Convert.ToInt32(options.GetPageParameterString("userRole"));
                     var repo = DependencyResolver.Current.GetService<INotificationService>();
                     string globalSearch = options.GetAdditionalQueryOptionString("search");
                     string sortColumn = options.GetSortColumnData<string>();
                     var items = repo.GetNotificationData(out totalRecords, options.GetLimitOffset(), options.GetLimitRowcount(),
                         sortColumn, options.SortDirection == SortDirection.Dsc, userId);
                     return new QueryResult<NotificationGridModel>()
                     {
                         Items = items,
                         TotalRecords = totalRecords
                     };
                 })
             );
            #endregion

            #region ArrangeTest
            AddToGridDictonary<ArrangeTestGridModel>("ArrangeTestGrid");
            MVCGridDefinitionTable.Add("ArrangeTestGrid",
                  new MVCGridBuilder<ArrangeTestGridModel>(gridDefaults, colDefauls).AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
                  .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                  .WithSorting(sorting: true, defaultSortColumn: "CreatedOn", defaultSortDirection: SortDirection.Dsc)
                  .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                  .WithAdditionalQueryOptionNames("search")
                  .WithFiltering(true)
                  .AddColumns(cols =>
                  {

                      cols.Add("Title").WithHeaderText("Title")
                            .WithVisibility(true, false)
                            .WithValueExpression(p => p.Title)
                            .WithFiltering(true);
                      cols.Add("SelectedClass").WithHeaderText("Class")
                      .WithVisibility(true, true)
                      .WithValueExpression(p => p.SelectedClass)
                      .WithFiltering(true);
                      cols.Add("SubjectName").WithHeaderText("Subject")
                      .WithVisibility(true, true)
                      .WithValueExpression(p => p.SubjectName)
                      .WithFiltering(true);
                      cols.Add("CreatedOn").WithHeaderText("Date")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.CreatedOn.ToString("dd/MM/yyyy"))
                          .WithFiltering(true);
                      cols.Add("TestType").WithHeaderText("Type")
                         .WithVisibility(true, true)
                         .WithValueExpression(p => p.TestType.ToString())
                         .WithFiltering(true);
                      cols.Add("Action").WithColumnName("Action")
                  .WithVisibility(true, false)
                     .WithSorting(false)
                     .WithHeaderText("Action")
                     .WithHtmlEncoding(false)
                     .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("DetailsArrengeTest", "Paper", new { id = p.ArrengeTestId }) + "'>Details</a>")
                     .WithValueTemplate("{Value}");
                  })
                  //.WithAdditionalSetting(MVCGrid.Rendering.BootstrapRenderingEngine.SettingNameTableClass, "notreal") // Example of changing table css class
                  .WithRetrieveDataMethod((context) =>
                  {
                      var options = context.QueryOptions;
                      int totalRecords;
                      var repo = DependencyResolver.Current.GetService<ITestPaperService>();
                      string globalSearch = options.GetAdditionalQueryOptionString("search");
                      string sortColumn = options.GetSortColumnData<string>();
                      var items = repo.GetArrangeTestData(out totalRecords, options.GetLimitOffset(), options.GetLimitRowcount(),
                          sortColumn, options.SortDirection == SortDirection.Dsc);
                      return new QueryResult<ArrangeTestGridModel>()
                      {
                          Items = items,
                          TotalRecords = totalRecords
                      };
                  })
              );
            #endregion

            #region StudentTimetable
            AddToGridDictonary<AttendanceGridModel>("StudentTimetableGrid");
            MVCGridDefinitionTable.Add("StudentTimetableGrid",
                 new MVCGridBuilder<StudentTimetableGridModel>(gridDefaults, colDefauls).AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
                 .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                 .WithSorting(sorting: true, defaultSortColumn: "Date", defaultSortDirection: SortDirection.Dsc)
                 .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                 .WithAdditionalQueryOptionNames("search")
                 .WithFiltering(true)
                  .WithPageParameterNames("userRole")
                       .AddColumns(cols =>
                       {
                           cols.Add("Description").WithHeaderText("Description")
                           .WithVisibility(true, false)
                           .WithValueExpression(p => p.Description.Replace("<br />", "\r\n"))
                           .WithFiltering(false);
                           cols.Add("Date").WithHeaderText("Date")
                            .WithVisibility(true, false)
                            .WithValueExpression(p => p.StudentTimetableDate.ToString("dd/MM/yyyy"));
                           cols.Add("Action").WithColumnName("Action")
                           .WithVisibility(true, false)
                           .WithSorting(false)
                           .WithHeaderText("Action")
                           .WithHtmlEncoding(false)
                           .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Details", "StudentTimetable", new { id = p.StudentTimetableId }) + "'>Details</a> | <a href='" + c.UrlHelper.Action("Delete", "StudentTimetable", new { id = p.StudentTimetableId }) + "'>Delete</a>| <a href='" + c.UrlHelper.Action("Edit", "StudentTimetable", new { id = p.StudentTimetableId }) + "'>Edit</a>")
                           .WithValueTemplate("{Value}");

                       })
                       .WithRetrieveDataMethod((context) =>
                       {
                           var options = context.QueryOptions;
                           int userId = Convert.ToInt16(options.GetPageParameterString("userRole"));
                           int totalRecords;
                           int ClientId = userId;
                           var repo = DependencyResolver.Current.GetService<IStudentTimetableService>();
                           string globalSearch = options.GetAdditionalQueryOptionString("search");
                           string sortColumn = options.GetSortColumnData<string>();
                           var items = repo.GetStudentExamTimetable(out totalRecords, userId, options.GetLimitOffset(), options.GetLimitRowcount(),
                          sortColumn, options.SortDirection == SortDirection.Dsc, ClientId);
                           return new QueryResult<StudentTimetableGridModel>()
                           {
                               Items = items,
                               TotalRecords = totalRecords
                           };
                       })
                   );
            #endregion

            #region DailyPracticePaper
            AddToGridDictonary<AttendanceGridModel>("DailyPracticePaperGrid");
            MVCGridDefinitionTable.Add("DailyPracticePaperGrid",
                 new MVCGridBuilder<DailyPracticePaperGridModel>(gridDefaults, colDefauls).AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
                 .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                 .WithSorting(sorting: true, defaultSortColumn: "Date", defaultSortDirection: SortDirection.Dsc)
                 .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                 .WithAdditionalQueryOptionNames("search")
                 .WithFiltering(true)
                 .WithPageParameterNames("userRole")
                       .AddColumns(cols =>
                       {
                           cols.Add("Daily Practice").WithHeaderText("Daily Practice")
                           .WithVisibility(true, false)
                           .WithValueExpression(p => p.Description.Replace("<br />", "\r\n"))
                           .WithFiltering(true);
                           cols.Add("Date").WithHeaderText("Date")
                           .WithVisibility(true, false)
                           .WithValueExpression(p => p.DailyPracticePaperDate.ToString("dd/MM/yyyy"));
                           cols.Add("Action").WithColumnName("Action")
                           .WithVisibility(true, false)
                           .WithSorting(false)
                           .WithHeaderText("Action")
                           .WithHtmlEncoding(false)
                           .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Details", "DailyPracticePaper", new { id = p.DailyPracticePaperId }) + "'>Details</a> | <a href='" + c.UrlHelper.Action("Delete", "DailyPracticePaper", new { id = p.DailyPracticePaperId }) + "'>Delete</a>| <a href='" + c.UrlHelper.Action("Edit", "DailyPracticePaper", new { id = p.DailyPracticePaperId }) + "'>Edit</a>")
                           .WithValueTemplate("{Value}");

                       })
                       .WithRetrieveDataMethod((context) =>
                       {
                           var options = context.QueryOptions;
                           int userId = Convert.ToInt16(options.GetPageParameterString("userRole"));
                           int totalRecords;
                           var repo = DependencyResolver.Current.GetService<IDailyPracticePaperService>();
                           string globalSearch = options.GetAdditionalQueryOptionString("search");
                           string sortColumn = options.GetSortColumnData<string>();
                           var items = repo.GetDailyPracticePaper(out totalRecords, userId, options.GetLimitOffset(), options.GetLimitRowcount(),
                          sortColumn, options.SortDirection == SortDirection.Dsc);
                           return new QueryResult<DailyPracticePaperGridModel>()
                           {
                               Items = items,
                               TotalRecords = totalRecords
                           };
                       })
                   );
            #endregion

            #region OfflineTestPaper
            AddToGridDictonary<OfflineTestPaperGridModel>("OfflineTestPaperGrid");
            MVCGridDefinitionTable.Add("OfflineTestPaperGrid",
                 new MVCGridBuilder<OfflineTestPaperGridModel>(gridDefaults, colDefauls).AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
                 .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                 .WithSorting(sorting: true, defaultSortColumn: "CreatedOn", defaultSortDirection: SortDirection.Dsc)
                 .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                 .WithAdditionalQueryOptionNames("search")
                 .WithFiltering(true)
                 .WithPageParameterNames("userRole")
                 .AddColumns(cols =>
                 {
                     //cols.Add("Id").WithValueExpression((p, c) => c.UrlHelper.Action("detail", "demo", new { id = p.UserId }))
                     //    .WithValueTemplate("<a href='{Value}'>{Model.Id}</a>", false)
                     //    .WithPlainTextValueExpression(p => p.Id.ToString());
                     cols.Add("Title").WithHeaderText("Title")
                          .WithVisibility(true, false)
                          .WithValueExpression(p => p.Title)
                          .WithFiltering(true);
                     cols.Add("Class").WithHeaderText("Class")
                      .WithVisibility(true, true)
                      .WithValueExpression(p => p.Class)
                      .WithFiltering(true);
                     cols.Add("Subject").WithHeaderText("Subject")
                     .WithVisibility(true, true)
                     .WithValueExpression(p => p.Subject)
                     .WithFiltering(true);
                     cols.Add("TestInTime").WithHeaderText("In Time")
                        .WithVisibility(true, true)
                        .WithValueExpression(p => p.TestInTime.ToString("hh:mm:ss") == "12:00:00" ? "-" : p.TestInTime.ToString("hh:mm tt"))
                        .WithFiltering(true);
                     cols.Add("TestOutTime").WithHeaderText("Out Time")
                        .WithVisibility(true, true)
                        .WithValueExpression(p => p.TestOutTime.ToString("hh:mm:ss") == "12:00:00" ? "-" : p.TestOutTime.ToString("hh:mm tt"))
                        .WithFiltering(true);
                     cols.Add("TotalMarks").WithHeaderText("Total Marks")
                        .WithVisibility(true, true)
                        .WithValueExpression(p => p.TotalMarks.ToString())
                        .WithFiltering(true);
                     cols.Add("TestDate").WithHeaderText("Test Date")
                        .WithVisibility(true, true)
                        .WithValueExpression(p => p.TestDate.ToString("dd/MM/yyyy"))
                         .WithFiltering(true);
                     cols.Add("CreatedOn").WithHeaderText("Date")
                      .WithVisibility(false, true)
                      .WithValueExpression(p => p.CreatedOn.ToString("dd/MM/yyyy"))
                       .WithFiltering(true);
                     cols.Add("Action").WithColumnName("Action")
                 .WithVisibility(true, false)
                    .WithSorting(false)
                    .WithHeaderText("Action")
                    .WithHtmlEncoding(false)
                    .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Edit", "OfflineTestPaper", new { id = p.OfflineTestPaperId }) + "'>Edit</a> | <a href='" + c.UrlHelper.Action("Delete", "OfflineTestPaper", new { id = p.OfflineTestPaperId }) + "'>Delete</a>")
                    .WithValueTemplate("{Value}");
                 })
                 //.WithAdditionalSetting(MVCGrid.Rendering.BootstrapRenderingEngine.SettingNameTableClass, "notreal") // Example of changing table css class
                 .WithRetrieveDataMethod((context) =>
                 {
                     var options = context.QueryOptions;
                     int totalRecords;
                     int userId = Convert.ToInt16(options.GetPageParameterString("userRole"));
                     var repo = DependencyResolver.Current.GetService<IOfflineTestPaper>();
                     string globalSearch = options.GetAdditionalQueryOptionString("search");
                     string sortColumn = options.GetSortColumnData<string>();
                     var items = repo.GetOfflineNotificationData(out totalRecords, userId, options.GetLimitOffset(), options.GetLimitRowcount(),
                         sortColumn, options.SortDirection == SortDirection.Dsc);
                     return new QueryResult<OfflineTestPaperGridModel>()
                     {
                         Items = items,
                         TotalRecords = totalRecords
                     };
                 })
             );
            #endregion

            #region OfflineTestPaperStudentMarks
            AddToGridDictonary<OfflineTestStudentMarksGridModel>("OfflineTestPaperStudentMarksGrid");
            MVCGridDefinitionTable.Add("OfflineTestPaperStudentMarksGrid",
                 new MVCGridBuilder<OfflineTestStudentMarksGridModel>(gridDefaults, colDefauls).AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
                 .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                 .WithSorting(sorting: true, defaultSortColumn: "CreatedOn", defaultSortDirection: SortDirection.Dsc)
                 .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                 .WithAdditionalQueryOptionNames("search")
                 .WithFiltering(true)
                 .WithPageParameterNames("userRole")
                 .AddColumns(cols =>
                 {
                     cols.Add("Title").WithHeaderText("Title")
                          .WithVisibility(true, false)
                          .WithValueExpression(p => p.Title)
                          .WithFiltering(true);
                     cols.Add("Class").WithHeaderText("Class")
                      .WithVisibility(true, true)
                      .WithValueExpression(p => p.Class)
                      .WithFiltering(true);
                     cols.Add("Subject").WithHeaderText("Subject")
                     .WithVisibility(true, true)
                     .WithValueExpression(p => p.Subject)
                     .WithFiltering(true);
                     cols.Add("TotalMarks").WithHeaderText("Total Marks")
                        .WithVisibility(true, true)
                        .WithValueExpression(p => p.TotalMarks.ToString())
                        .WithFiltering(true);
                     cols.Add("Date").WithHeaderText("Date")
                      .WithVisibility(false, true)
                      .WithValueExpression(p => p.Date.ToString("dd/MM/yyyy"))
                       .WithFiltering(true);
                     cols.Add("CreatedOn").WithHeaderText("CreatedOn")
                    .WithVisibility(false, false)
                    .WithValueExpression(p => p.CreatedOn.ToString("dd/MM/yyyy"))
                     .WithFiltering(true);
                     cols.Add("Action").WithColumnName("Action")
                    .WithVisibility(true, false)
                    .WithSorting(false)
                    .WithHeaderText("Action")
                    .WithHtmlEncoding(false)
                    .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Edit", "UploadOfflineMarks", new { id = p.OfflineTestPaperId }) + "'>Edit</a> | <a href='" + c.UrlHelper.Action("Delete", "UploadOfflineMarks", new { id = p.OfflineTestPaperId }) + "'>Delete</a> | <a href='" + c.UrlHelper.Action("Details", "UploadOfflineMarks", new { id = p.OfflineTestPaperId }) + "'>Details</a>")
                    .WithValueTemplate("{Value}");
                 })
                 //.WithAdditionalSetting(MVCGrid.Rendering.BootstrapRenderingEngine.SettingNameTableClass, "notreal") // Example of changing table css class
                 .WithRetrieveDataMethod((context) =>
                 {
                     var options = context.QueryOptions;
                     int totalRecords;
                     int userId = Convert.ToInt16(options.GetPageParameterString("userRole"));
                     var repo = DependencyResolver.Current.GetService<IOfflineTestStudentMarksService>();
                     string globalSearch = options.GetAdditionalQueryOptionString("search");
                     string sortColumn = options.GetSortColumnData<string>();
                     var items = repo.GetOfflineNotificationData(out totalRecords, userId, options.GetLimitOffset(), options.GetLimitRowcount(),
                         sortColumn, options.SortDirection == SortDirection.Dsc);
                     return new QueryResult<OfflineTestStudentMarksGridModel>()
                     {
                         Items = items,
                         TotalRecords = totalRecords
                     };
                 })
             );
            #endregion

            #region StudentTimetableclass
            AddToGridDictonary<AttendanceGridModel>("StudentClassTimetableGrid");
            MVCGridDefinitionTable.Add("StudentClassTimetableGrid",
                 new MVCGridBuilder<StudentTimetableGridModel>(gridDefaults, colDefauls).AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
                 .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                 .WithSorting(sorting: true, defaultSortColumn: "Date", defaultSortDirection: SortDirection.Dsc)
                 .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                 .WithAdditionalQueryOptionNames("search")
                 .WithFiltering(true)
                  .WithPageParameterNames("userRole")
                       .AddColumns(cols =>
                       {
                           cols.Add("Description").WithHeaderText("Description")
                           .WithVisibility(true, false)
                           .WithValueExpression(p => p.Description.Replace("<br />", "\r\n"))
                           .WithFiltering(false);
                           cols.Add("Date").WithHeaderText("Date")
                            .WithVisibility(true, false)
                            .WithValueExpression(p => p.StudentTimetableDate.ToString("dd/MM/yyyy"));
                           cols.Add("Action").WithColumnName("Action")
                           .WithVisibility(true, false)
                           .WithSorting(false)
                           .WithHeaderText("Action")
                           .WithHtmlEncoding(false)
                             .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Details", "StudentClassTimetable", new { id = p.StudentTimetableId }) + "'>Details</a> | <a href='" + c.UrlHelper.Action("Delete", "StudentClassTimetable", new { id = p.StudentTimetableId }) + "'>Delete</a>| <a href='" + c.UrlHelper.Action("Edit", "StudentClassTimetable", new { id = p.StudentTimetableId }) + "'>Edit</a>")
                           .WithValueTemplate("{Value}");

                       })
                       .WithRetrieveDataMethod((context) =>
                       {
                           var options = context.QueryOptions;
                           int userId = Convert.ToInt16(options.GetPageParameterString("userRole"));
                           int totalRecords;
                           int ClientId = userId;
                           var repo = DependencyResolver.Current.GetService<IStudentTimetableService>();
                           string globalSearch = options.GetAdditionalQueryOptionString("search");
                           string sortColumn = options.GetSortColumnData<string>();
                           var items = repo.GetStudentClassTimetable(out totalRecords, userId, options.GetLimitOffset(), options.GetLimitRowcount(),
                          sortColumn, options.SortDirection == SortDirection.Dsc, ClientId);
                           return new QueryResult<StudentTimetableGridModel>()
                           {
                               Items = items,
                               TotalRecords = totalRecords
                           };
                       })
                   );
            #endregion

            #region StudentAdmission
            AddToGridDictonary<AttendanceGridModel>("StudentAdmissionGrid");
            MVCGridDefinitionTable.Add("StudentAdmissionGrid",
                 new MVCGridBuilder<AttendanceGridModel>(gridDefaults, colDefauls).AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
                 .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                 .WithSorting(sorting: true, defaultSortColumn: "Date", defaultSortDirection: SortDirection.Dsc)
                 .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                 .WithAdditionalQueryOptionNames("search")
                 .WithFiltering(true)
                 .WithPageParameterNames("classId")
                 .WithPageParameterNames("branchId")
                 .WithPageParameterNames("studentBatches")
                 .WithPageParameterNames("sId")
                 .AddColumns(cols =>
                 {
                     cols.Add("BranchName").WithHeaderText("BranchName")
                         .WithVisibility(true, false)
                         .WithValueExpression(p => p.BranchName)
                         .WithFiltering(true);
                     cols.Add("ClassName").WithHeaderText("ClassName")
                         .WithVisibility(true, false)
                         .WithValueExpression(p => p.ClassName)
                         .WithFiltering(true);
                     cols.Add("BatchName").WithHeaderText("BatchName")
                         .WithVisibility(true, false)
                         .WithValueExpression(p => p.BatchName)
                         .WithFiltering(true);
                     cols.Add("TeacherName").WithHeaderText("TeacherName")
                         .WithVisibility(true, false)
                         .WithValueExpression(p => p.TeacherName)
                         .WithFiltering(true);
                     cols.Add("Activity").WithHeaderText("Activity")
                         .WithVisibility(true, false)
                         .WithValueExpression(p => p.Activity)
                         .WithFiltering(true);
                     cols.Add("Date").WithHeaderText("Date")
                         .WithVisibility(true, false)
                         .WithValueExpression(p => p.Date.ToString("dd/MM/yyyy"))
                         .WithFiltering(true);
                     cols.Add("StudentAttendence").WithHeaderText("status")
                      .WithVisibility(true, false)
                      .WithValueExpression(p => p.StudentAttendence)
                      .WithFiltering(true);
                 })
                 .WithRetrieveDataMethod((context) =>
                 {
                     var options = context.QueryOptions;
                     int totalRecords;
                     int classId = Convert.ToInt16(options.GetPageParameterString("classId"));
                     int branchId = Convert.ToInt16(options.GetPageParameterString("branchId"));
                     int studentBatches = Convert.ToInt16(options.GetPageParameterString("studentBatches"));
                     int sId = Convert.ToInt16(options.GetPageParameterString("sId"));
                     var repo = DependencyResolver.Current.GetService<IAttendanceService>();
                     string globalSearch = options.GetAdditionalQueryOptionString("search");
                     string sortColumn = options.GetSortColumnData<string>();
                     var items = repo.GetStudentAttendanceData(out totalRecords, classId, branchId, studentBatches, sId, Convert.ToDateTime(options.GetFilterString("Date")),
                          options.GetFilterString("TeacherName"), options.GetLimitOffset(), options.GetLimitRowcount(), sortColumn, options.SortDirection == SortDirection.Dsc);
                     return new QueryResult<AttendanceGridModel>()
                     {
                         Items = items,
                         TotalRecords = totalRecords
                     };
                 })
             );
            #endregion

            #region ArrangeTestResult
            AddToGridDictonary<ArrangeTestResultGridModel>("ArrangeTestResultGrid");
            MVCGridDefinitionTable.Add("ArrangeTestResultGrid",
                new MVCGridBuilder<ArrangeTestResultGridModel>(gridDefaults, colDefauls).AddRenderingEngine("csv", typeof(CustomCsvRenderingEngine))
                .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                .WithSorting(sorting: true, defaultSortColumn: "CreatedOn", defaultSortDirection: SortDirection.Dsc)
                .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                .WithAdditionalQueryOptionNames("search")
                .WithFiltering(true)
                .WithPageParameterNames("userRole")
                .AddColumns(cols =>
                {
                    cols.Add("StudentName").WithHeaderText("Student Name")
                        .WithVisibility(true, false)
                        .WithValueExpression(p => p.StudentName)
                        .WithFiltering(true);
                    cols.Add("TestPaperTitle").WithHeaderText("Test Paper Title")
                        .WithVisibility(true, false)
                        .WithValueExpression(p => p.TestPaperTitle)
                        .WithFiltering(true);
                    cols.Add("TestDate").WithHeaderText("Test Date")
                        .WithVisibility(true, false)
                        .WithValueExpression(p => p.TestDate.ToString("dd/MM/yyyy"))
                        .WithFiltering(true);
                    cols.Add("ObtainedMarks").WithHeaderText("Obtained Marks")
                        .WithVisibility(true, false)
                        .WithValueExpression(p => p.ObtainedMarks.ToString())
                        .WithFiltering(true);
                    cols.Add("OutOfMarks").WithHeaderText("OutOf Marks")
                        .WithVisibility(true, false)
                        .WithValueExpression(p => p.OutOfMarks.ToString())
                        .WithFiltering(true);
                    cols.Add("CreatedOn").WithHeaderText("CreatedOn")
                        .WithVisibility(false, false)
                        .WithValueExpression(p => p.CreatedOn.ToString())
                        .WithFiltering(true);
                    cols.Add("ClassId").WithHeaderText("ClassId")
                        .WithVisibility(false, false)
                        .WithValueExpression(p => p.ClassId.ToString())
                        .WithFiltering(true);
                    cols.Add("TestPaperId").WithHeaderText("TestPaperId")
                        .WithVisibility(false, false)
                        .WithValueExpression(p => p.TestPaperId.ToString())
                        .WithFiltering(true);
                    cols.Add("Action").WithColumnName("Action")
                                      .WithVisibility(true, false)
                                         .WithSorting(false)
                                         .WithHeaderText("Action")
                                         .WithHtmlEncoding(false)
                                         .WithValueExpression((p, c) => "<a href='" + c.UrlHelper.Action("Details", "ArrangeTestResult", new { id = p.ArrangeTestResultId }) + "'>Details</a>")
                                         .WithValueTemplate("{Value}");
                })
                .WithRetrieveDataMethod((context) =>
                {
                    var options = context.QueryOptions;
                    int userId = Convert.ToInt32(options.GetPageParameterString("userRole"));
                    int totalRecords;
                    var repo = DependencyResolver.Current.GetService<IArrangeTestResultService>();
                    string globalSearch = options.GetAdditionalQueryOptionString("search");
                    string sortColumn = options.GetSortColumnData<string>();
                    var items = repo.GetArrangeTestPaperResultData(out totalRecords, userId, Convert.ToInt32(options.GetFilterString("ClassId")),
                        Convert.ToInt32(options.GetFilterString("TestPaperId")), options.GetLimitOffset(), options.GetLimitRowcount(),
                        sortColumn, options.SortDirection == SortDirection.Dsc);
                    return new QueryResult<ArrangeTestResultGridModel>()
                    {
                        Items = items,
                        TotalRecords = totalRecords
                    };
                })
            );
            #endregion


        }

        public static void AddToGridDictonary<T>(string gridName) where T : class
        {
            if (!GridDictonary.ContainsKey(gridName))
            {
                GridDictonary.Add(gridName, typeof(T).FullName);
            }
        }
    }
}
