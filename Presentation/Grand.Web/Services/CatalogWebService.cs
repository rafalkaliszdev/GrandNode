﻿using Grand.Core;
using Grand.Core.Caching;
using Grand.Core.Domain.Blogs;
using Grand.Core.Domain.Catalog;
using Grand.Core.Domain.Common;
using Grand.Core.Domain.Forums;
using Grand.Core.Domain.Media;
using Grand.Core.Domain.Vendors;
using Grand.Services.Catalog;
using Grand.Services.Common;
using Grand.Services.Customers;
using Grand.Services.Directory;
using Grand.Services.Events;
using Grand.Services.Localization;
using Grand.Services.Media;
using Grand.Services.Security;
using Grand.Services.Seo;
using Grand.Services.Stores;
using Grand.Services.Topics;
using Grand.Services.Vendors;
using Grand.Web.Extensions;
using Grand.Web.Framework.Events;
using Grand.Web.Infrastructure.Cache;
using Grand.Web.Models.Catalog;
using Grand.Web.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
///*using System.Web;*/
///*using System.Web.Mvc;*/

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace Grand.Web.Services
{
    public partial class CatalogWebService : ICatalogWebService
    {
        private readonly IWebHelper _webHelper;
        private readonly IProductWebService _productWebService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICacheManager _cacheManager;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductService _productService;
        private readonly ITopicService _topicService;
        private readonly IPictureService _pictureService;
        private readonly IVendorService _vendorService;
        private readonly IProductTagService _productTagService;
        private readonly ICurrencyService _currencyService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISearchTermService _searchTermService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IAclService _aclService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ICategoryTemplateService _categoryTemplateService;
        private readonly IManufacturerTemplateService _manufacturerTemplateService;
        private readonly IPriceFormatter _priceFormatter;

        private readonly CatalogSettings _catalogSettings;
        private readonly BlogSettings _blogSettings;
        private readonly ForumSettings _forumSettings;
        private readonly MenuItemSettings _menuItemSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly VendorSettings _vendorSettings;

        public CatalogWebService(
            IWebHelper webHelper,
            IProductWebService productWebService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IStoreContext storeContext,
            //ICacheManager cacheManager,
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IProductService productService,
            ITopicService topicService,
            //IPictureService pictureService,
            IVendorService vendorService,
            IProductTagService productTagService,
            ICurrencyService currencyService,
            IHttpContextAccessor httpContextAccessor,
            ISearchTermService searchTermService,
            IEventPublisher eventPublisher,
            IAclService aclService,
            IPermissionService permissionService,
            IStoreMappingService storeMappingService,
            ISpecificationAttributeService specificationAttributeService,
            ICategoryTemplateService categoryTemplateService,
            IManufacturerTemplateService manufacturerTemplateService,
            IPriceFormatter priceFormatter,

            CatalogSettings catalogSettings,
            BlogSettings blogSettings,
            ForumSettings forumSettings,
            MenuItemSettings menuItemSettings,
            MediaSettings mediaSettings,
            VendorSettings vendorSettings
            )
        {
            this._webHelper = webHelper;
            this._productWebService = productWebService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            //this._cacheManager = cacheManager;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._productService = productService;
            this._topicService = topicService;
            //this._pictureService = pictureService;
            this._vendorService = vendorService;
            this._productTagService = productTagService;
            this._currencyService = currencyService;
            this._httpContextAccessor = httpContextAccessor;
            this._searchTermService = searchTermService;
            this._eventPublisher = eventPublisher;
            this._aclService = aclService;
            this._permissionService = permissionService;
            this._storeMappingService = storeMappingService;
            this._specificationAttributeService = specificationAttributeService;
            this._categoryTemplateService = categoryTemplateService;
            this._manufacturerTemplateService = manufacturerTemplateService;
            this._priceFormatter = priceFormatter;

            this._catalogSettings = catalogSettings;
            this._blogSettings = blogSettings;
            this._forumSettings = forumSettings;
            this._menuItemSettings = menuItemSettings;
            this._mediaSettings = mediaSettings;
            this._vendorSettings = vendorSettings;
        }

        #region Utilities
        //ctl hope it wont make difference
        //[NonAction]
        protected virtual List<string> GetChildCategoryIds(string parentCategoryId)
        {
            string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_CHILD_IDENTIFIERS_MODEL_KEY,
                parentCategoryId,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            //return _cacheManager.Get(cacheKey, () =>
            //{
                var categoriesIds = new List<string>();
                var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId);
                foreach (var category in categories)
                {
                    categoriesIds.Add(category.Id);
                    categoriesIds.AddRange(GetChildCategoryIds(category.Id));
                }
                return categoriesIds;
            //});
        }

        #endregion

        #region Sort,view,size options

        public virtual void PrepareSortingOptions(CatalogPagingFilteringModel pagingFilteringModel, CatalogPagingFilteringModel command)
        {
            if (pagingFilteringModel == null)
                throw new ArgumentNullException("pagingFilteringModel");

            if (command == null)
                throw new ArgumentNullException("command");

            var allDisabled = _catalogSettings.ProductSortingEnumDisabled.Count == Enum.GetValues(typeof(ProductSortingEnum)).Length;
            pagingFilteringModel.AllowProductSorting = _catalogSettings.AllowProductSorting && !allDisabled;

            var activeOptions = Enum.GetValues(typeof(ProductSortingEnum)).Cast<int>()
                .Except(_catalogSettings.ProductSortingEnumDisabled)
                .Select((idOption) =>
                {
                    int order;
                    return new KeyValuePair<int, int>(idOption, _catalogSettings.ProductSortingEnumDisplayOrder.TryGetValue(idOption, out order) ? order : idOption);
                })
                .OrderBy(x => x.Value);
            if (command.OrderBy == null)
                command.OrderBy = allDisabled ? 0 : activeOptions.First().Key;

            if (pagingFilteringModel.AllowProductSorting)
            {
                foreach (var option in activeOptions)
                {
                    var currentPageUrl = _webHelper.GetThisPageUrl(true);
                    var sortUrl = _webHelper.ModifyQueryString(currentPageUrl, "orderby=" + (option.Key).ToString(), null);

                    var sortValue = ((ProductSortingEnum)option.Key).GetLocalizedEnum(_localizationService, _workContext);
                    pagingFilteringModel.AvailableSortOptions.Add(new SelectListItem
                    {
                        Text = sortValue,
                        Value = sortUrl,
                        Selected = option.Key == command.OrderBy
                    });
                }
            }
        }
        public virtual void PrepareViewModes(CatalogPagingFilteringModel pagingFilteringModel, CatalogPagingFilteringModel command)
        {
            if (pagingFilteringModel == null)
                throw new ArgumentNullException("pagingFilteringModel");

            if (command == null)
                throw new ArgumentNullException("command");

            pagingFilteringModel.AllowProductViewModeChanging = _catalogSettings.AllowProductViewModeChanging;

            var viewMode = !string.IsNullOrEmpty(command.ViewMode)
                ? command.ViewMode
                : _catalogSettings.DefaultViewMode;
            pagingFilteringModel.ViewMode = viewMode;
            if (pagingFilteringModel.AllowProductViewModeChanging)
            {
                var currentPageUrl = _webHelper.GetThisPageUrl(true);
                //grid
                pagingFilteringModel.AvailableViewModes.Add(new SelectListItem
                {
                    Text = _localizationService.GetResource("Catalog.ViewMode.Grid"),
                    Value = _webHelper.ModifyQueryString(currentPageUrl, "viewmode=grid", null),
                    Selected = viewMode == "grid"
                });
                //list
                pagingFilteringModel.AvailableViewModes.Add(new SelectListItem
                {
                    Text = _localizationService.GetResource("Catalog.ViewMode.List"),
                    Value = _webHelper.ModifyQueryString(currentPageUrl, "viewmode=list", null),
                    Selected = viewMode == "list"
                });
            }
        }

        public virtual void PreparePageSizeOptions(CatalogPagingFilteringModel pagingFilteringModel, CatalogPagingFilteringModel command,
            bool allowCustomersToSelectPageSize, string pageSizeOptions, int fixedPageSize)
        {
            if (pagingFilteringModel == null)
                throw new ArgumentNullException("pagingFilteringModel");

            if (command == null)
                throw new ArgumentNullException("command");

            if (command.PageNumber <= 0)
            {
                command.PageNumber = 1;
            }
            pagingFilteringModel.AllowCustomersToSelectPageSize = false;
            if (allowCustomersToSelectPageSize && pageSizeOptions != null)
            {
                var pageSizes = pageSizeOptions.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (pageSizes.Any())
                {
                    // get the first page size entry to use as the default (category page load) or if customer enters invalid value via query string
                    if (command.PageSize <= 0 || !pageSizes.Contains(command.PageSize.ToString()))
                    {
                        int temp;
                        if (int.TryParse(pageSizes.FirstOrDefault(), out temp))
                        {
                            if (temp > 0)
                            {
                                command.PageSize = temp;
                            }
                        }
                    }

                    var currentPageUrl = _webHelper.GetThisPageUrl(true);
                    var sortUrl = _webHelper.ModifyQueryString(currentPageUrl, "pagesize={0}", null);
                    sortUrl = _webHelper.RemoveQueryString(sortUrl, "pagenumber");

                    foreach (var pageSize in pageSizes)
                    {
                        int temp;
                        if (!int.TryParse(pageSize, out temp))
                        {
                            continue;
                        }
                        if (temp <= 0)
                        {
                            continue;
                        }

                        pagingFilteringModel.PageSizeOptions.Add(new SelectListItem
                        {
                            Text = pageSize,
                            Value = String.Format(sortUrl, pageSize),
                            Selected = pageSize.Equals(command.PageSize.ToString(), StringComparison.OrdinalIgnoreCase)
                        });
                    }

                    if (pagingFilteringModel.PageSizeOptions.Any())
                    {
                        pagingFilteringModel.PageSizeOptions = pagingFilteringModel.PageSizeOptions.OrderBy(x => int.Parse(x.Text)).ToList();
                        pagingFilteringModel.AllowCustomersToSelectPageSize = true;

                        if (command.PageSize <= 0)
                        {
                            command.PageSize = int.Parse(pagingFilteringModel.PageSizeOptions.FirstOrDefault().Text);
                        }
                    }
                }
            }
            else
            {
                //customer is not allowed to select a page size
                command.PageSize = fixedPageSize;
            }

            //ensure pge size is specified
            if (command.PageSize <= 0)
            {
                command.PageSize = fixedPageSize;
            }
        }

        #endregion

        #region Category

        public virtual List<CategorySimpleModel> PrepareCategorySimpleModels()
        {
            //string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_ALL_MODEL_KEY,
            //    _workContext.WorkingLanguage.Id,
            //    string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
            //    _storeContext.CurrentStore.Id);
            //return _cacheManager.Get(cacheKey, () => PrepareCategorySimpleModels(""));
            return PrepareCategorySimpleModels("");


        }

        public virtual List<CategorySimpleModel> PrepareCategorySimpleModels(string rootCategoryId,
            bool loadSubCategories = true, IList<Category> allCategories = null)
        {
            var result = new List<CategorySimpleModel>();

            if (allCategories == null)
            {
                allCategories = _categoryService.GetAllCategories(storeId: _storeContext.CurrentStore.Id);
            }
            var categories = allCategories.Where(c => c.ParentCategoryId == rootCategoryId).ToList();
            foreach (var category in categories)
            {
                var categoryModel = new CategorySimpleModel
                {
                    Id = category.Id,
                    Name = category.GetLocalized(x => x.Name),
                    SeName = category.GetSeName(),
                    IncludeInTopMenu = category.IncludeInTopMenu
                };

                ////product number for each category
                //if (_catalogSettings.ShowCategoryProductNumber)
                //{
                //    string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_NUMBER_OF_PRODUCTS_MODEL_KEY,
                //        string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                //        _storeContext.CurrentStore.Id,
                //        category.Id);
                //    categoryModel.NumberOfProducts = _cacheManager.Get(cacheKey, () =>
                //    {
                //        var categoryIds = new List<string>();
                //        categoryIds.Add(category.Id);
                //        //include subcategories
                //        if (_catalogSettings.ShowCategoryProductNumberIncludingSubcategories)
                //            categoryIds.AddRange(GetChildCategoryIds(category.Id));
                //        return _productService.GetCategoryProductNumber(categoryIds, _storeContext.CurrentStore.Id);
                //    });
                //}

                if (loadSubCategories)
                {
                    var subCategories = PrepareCategorySimpleModels(category.Id, loadSubCategories, allCategories);
                    categoryModel.SubCategories.AddRange(subCategories);
                }
                result.Add(categoryModel);
            }

            return result;
        }

        public virtual CategoryNavigationModel PrepareCategoryNavigation(string currentCategoryId, string currentProductId)
        {
            //get active category
            string activeCategoryId = "";
            if (!String.IsNullOrEmpty(currentCategoryId))
            {
                //category details page
                activeCategoryId = currentCategoryId;
            }
            else if (!String.IsNullOrEmpty(currentProductId))
            {
                //product details page
                var productCategories = _productService.GetProductById(currentProductId).ProductCategories; //_categoryService.GetProductCategoriesByProductId(currentProductId);
                if (productCategories.Any())
                    activeCategoryId = productCategories.FirstOrDefault().CategoryId;
            }
            var cachedModel = PrepareCategorySimpleModels();
            var model = new CategoryNavigationModel
            {
                CurrentCategoryId = activeCategoryId,
                Categories = cachedModel
            };

            return model;
        }

        public virtual string PrepareCategoryTemplateViewPath(string templateId)
        {
            var templateCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_TEMPLATE_MODEL_KEY, templateId);
            //var templateViewPath = _cacheManager.Get(templateCacheKey, () =>
            //{
                var template = _categoryTemplateService.GetCategoryTemplateById(templateId);
                if (template == null)
                    template = _categoryTemplateService.GetAllCategoryTemplates().FirstOrDefault();
                if (template == null)
                    throw new Exception("No default template could be loaded");
            //    return template.ViewPath;
            var templateViewPath = template.ViewPath;
            //});

            return templateViewPath;
        }

        public virtual CategoryModel PrepareCategory(Category category, CatalogPagingFilteringModel command)
        {
            var model = category.ToModel();

            //sorting
            PrepareSortingOptions(model.PagingFilteringContext, command);
            //view mode
            PrepareViewModes(model.PagingFilteringContext, command);
            //page size
            PreparePageSizeOptions(model.PagingFilteringContext, command,
                category.AllowCustomersToSelectPageSize,
                category.PageSizeOptions,
                category.PageSize);

            //price ranges
            model.PagingFilteringContext.PriceRangeFilter.LoadPriceRangeFilters(category.PriceRanges, _webHelper, _priceFormatter);
            var selectedPriceRange = model.PagingFilteringContext.PriceRangeFilter.GetSelectedPriceRange(_webHelper, category.PriceRanges);
            decimal? minPriceConverted = null;
            decimal? maxPriceConverted = null;
            if (selectedPriceRange != null)
            {
                if (selectedPriceRange.From.HasValue)
                    minPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(selectedPriceRange.From.Value, _workContext.WorkingCurrency);

                if (selectedPriceRange.To.HasValue)
                    maxPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(selectedPriceRange.To.Value, _workContext.WorkingCurrency);
            }


            //category breadcrumb
            if (_catalogSettings.CategoryBreadcrumbEnabled)
            {
                model.DisplayCategoryBreadcrumb = true;

                string breadcrumbCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_BREADCRUMB_KEY,
                    category.Id,
                    string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                    _storeContext.CurrentStore.Id,
                    _workContext.WorkingLanguage.Id);
                model.CategoryBreadcrumb =/* _cacheManager.Get(breadcrumbCacheKey, () =>*/
                    category
                    .GetCategoryBreadCrumb(_categoryService, _aclService, _storeMappingService)
                    .Select(catBr => new CategoryModel
                    {
                        Id = catBr.Id,
                        Name = catBr.GetLocalized(x => x.Name),
                        SeName = catBr.GetSeName()
                    })
                    .ToList();
                //);
            }

            //subcategories
            TemporaryWorkaround(category, model);
            //string subCategoriesCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_SUBCATEGORIES_KEY,
            //    category.Id,
            //    string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
            //    _storeContext.CurrentStore.Id,
            //    _workContext.WorkingLanguage.Id,
            //    _webHelper.IsCurrentConnectionSecured());
            //model.SubCategories = _cacheManager.Get(subCategoriesCacheKey, () =>
            //    _categoryService.GetAllCategoriesByParentCategoryId(category.Id)
            //    .Select(x =>
            //    {
            //        var subCatModel = new CategoryModel.SubCategoryModel
            //        {
            //            Id = x.Id,
            //            Name = x.GetLocalized(y => y.Name),
            //            SeName = x.GetSeName(),
            //            Description = x.GetLocalized(y => y.Description)
            //        };

            //        //prepare picture model
            //        int pictureSize = _mediaSettings.CategoryThumbPictureSize;
            //        var categoryPictureCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_PICTURE_MODEL_KEY, x.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
            //        subCatModel.PictureModel = _cacheManager.Get(categoryPictureCacheKey, () =>
            //        {
            //            var picture = _pictureService.GetPictureById(x.PictureId);
            //            var pictureModel = new PictureModel
            //            {
            //                FullSizeImageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.ApplyWatermarkForCategory),
            //                ImageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.ApplyWatermarkForCategory, pictureSize),
            //                Title = string.Format(_localizationService.GetResource("Media.Category.ImageLinkTitleFormat"), subCatModel.Name),
            //                AlternateText = string.Format(_localizationService.GetResource("Media.Category.ImageAlternateTextFormat"), subCatModel.Name)
            //            };
            //            return pictureModel;
            //        });

            //        return subCatModel;
            //    })
            //    .ToList()
            //);
            
            //featured products
            if (!_catalogSettings.IgnoreFeaturedProducts)
            {
                //We cache a value indicating whether we have featured products
                IPagedList<Product> featuredProducts = null;
                string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_HAS_FEATURED_PRODUCTS_KEY, category.Id,
                    string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()), _storeContext.CurrentStore.Id);
                bool? hasFeaturedProductsCache = null;// _cacheManager.Get<bool?>(cacheKey);
                if (!hasFeaturedProductsCache.HasValue)
                {
                    //no value in the cache yet
                    //let's load products and cache the result (true/false)
                    featuredProducts = _productService.SearchProducts(
                       pageSize: _catalogSettings.LimitOfFeaturedProducts,
                       categoryIds: new List<string> { category.Id },
                       storeId: _storeContext.CurrentStore.Id,
                       visibleIndividuallyOnly: true,
                       featuredProducts: true);
                    hasFeaturedProductsCache = featuredProducts.Any();
                    //_cacheManager.Set(cacheKey, hasFeaturedProductsCache, 60);
                }
                if (hasFeaturedProductsCache.Value && featuredProducts == null)
                {
                    //cache indicates that the category has featured products
                    //let's load them
                    featuredProducts = _productService.SearchProducts(
                       pageSize: _catalogSettings.LimitOfFeaturedProducts,
                       categoryIds: new List<string> { category.Id },
                       storeId: _storeContext.CurrentStore.Id,
                       visibleIndividuallyOnly: true,
                       featuredProducts: true);
                }
                if (featuredProducts != null)
                {
                    model.FeaturedProducts = _productWebService.PrepareProductOverviewModels(featuredProducts).ToList();
                }
            }


            var categoryIds = new List<string>();
            categoryIds.Add(category.Id);
            if (_catalogSettings.ShowProductsFromSubcategories)
            {
                //include subcategories
                categoryIds.AddRange(GetChildCategoryIds(category.Id));
            }
            //products
            IList<string> alreadyFilteredSpecOptionIds = model.PagingFilteringContext.SpecificationFilter.GetAlreadyFilteredSpecOptionIds(_webHelper);
            IList<string> filterableSpecificationAttributeOptionIds;
            var products = _productService.SearchProducts(out filterableSpecificationAttributeOptionIds, true,
                categoryIds: categoryIds,
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true,
                featuredProducts: _catalogSettings.IncludeFeaturedProductsInNormalLists ? null : (bool?)false,
                priceMin: minPriceConverted,
                priceMax: maxPriceConverted,
                filteredSpecs: alreadyFilteredSpecOptionIds,
                orderBy: (ProductSortingEnum)command.OrderBy,
                pageIndex: command.PageNumber - 1,
                pageSize: command.PageSize);
            model.Products = _productWebService.PrepareProductOverviewModels(products).ToList();

            model.PagingFilteringContext.LoadPagedList(products);

            //specs
            model.PagingFilteringContext.SpecificationFilter.PrepareSpecsFilters(alreadyFilteredSpecOptionIds,
                filterableSpecificationAttributeOptionIds,
                _specificationAttributeService, _webHelper, _workContext, _cacheManager);

            return model;
        }

        private void TemporaryWorkaround(Category category, CategoryModel model)
        {



            model.SubCategories = _categoryService.GetAllCategoriesByParentCategoryId(category.Id)
                .Select(x =>
                {
                    var subCatModel = new CategoryModel.SubCategoryModel
                    {
                        Id = x.Id,
                        Name = x.GetLocalized(y => y.Name),
                        SeName = x.GetSeName(),
                        Description = x.GetLocalized(y => y.Description)
                    };

                    //prepare picture model
                    int pictureSize = _mediaSettings.CategoryThumbPictureSize;


                    //var picture = _pictureService.GetPictureById(x.PictureId);
                    var pictureModel = new PictureModel
                    {
                        FullSizeImageUrl = "https://img0.etsystatic.com/144/0/11767653/il_570xN.1174170726_p07o.jpg",// _pictureService.GetPictureUrl(picture, _mediaSettings.ApplyWatermarkForCategory),
                        ImageUrl = "https://img0.etsystatic.com/144/0/11767653/il_570xN.1174170726_p07o.jpg",//_pictureService.GetPictureUrl(picture, _mediaSettings.ApplyWatermarkForCategory, pictureSize),
                        Title = string.Format(_localizationService.GetResource("Media.Category.ImageLinkTitleFormat"), subCatModel.Name),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Category.ImageAlternateTextFormat"), subCatModel.Name)
                    };
                    subCatModel.PictureModel = pictureModel;

                    return subCatModel;
                })
                .ToList();
        }

        public virtual List<CategoryModel> PrepareHomepageCategory()
        {
            //return new List<CategoryModel>() { new CategoryModel { Name = "qqqqq1 zglaszam odbiur" } };




            //string categoriesCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_HOMEPAGE_KEY,
            //    string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
            //    _storeContext.CurrentStore.Id,
            //    _workContext.WorkingLanguage.Id,
            //    _webHelper.IsCurrentConnectionSecured());

            var model = /*_cacheManager.Get(categoriesCacheKey, () =>*/
                _categoryService.GetAllCategoriesDisplayedOnHomePage()
                .Select(x =>
                {
                    var catModel = x.ToModel();

                    //prepare picture model
                    int pictureSize = _mediaSettings.CategoryThumbPictureSize;
                    var categoryPictureCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_PICTURE_MODEL_KEY, x.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);


                    //uncomment later
                    //catModel.PictureModel = _cacheManager.Get(categoryPictureCacheKey, () =>
                    //{
                    //    var picture = _pictureService.GetPictureById(x.PictureId);
                    //    var pictureModel = new PictureModel
                    //    {
                    //        FullSizeImageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.ApplyWatermarkForCategory),
                    //        ImageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.ApplyWatermarkForCategory, pictureSize),
                    //        Title = string.Format(_localizationService.GetResource("Media.Category.ImageLinkTitleFormat"), catModel.Name),
                    //        AlternateText = string.Format(_localizationService.GetResource("Media.Category.ImageAlternateTextFormat"), catModel.Name)
                    //    };
                    //    return pictureModel;
                    //});


                    //var picture = _pictureService.GetPictureById(x.PictureId);
                    catModel.PictureModel = new PictureModel
                    {
                        FullSizeImageUrl = "http://czerwonyobcas.pl/wp-content/uploads/2015/08/pude%C5%82ko.png",// _pictureService.GetPictureUrl(picture, _mediaSettings.ApplyWatermarkForCategory),
                        ImageUrl = "http://czerwonyobcas.pl/wp-content/uploads/2015/08/pude%C5%82ko.png",// _pictureService.GetPictureUrl(picture, _mediaSettings.ApplyWatermarkForCategory, pictureSize),
                        Title = string.Format(_localizationService.GetResource("Media.Category.ImageLinkTitleFormat"), catModel.Name),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Category.ImageAlternateTextFormat"), catModel.Name)
                    };

                    return catModel;
                })
                .ToList();
            //);
            return model;
        }

        #endregion

        #region Top menu

        public virtual TopMenuModel PrepareTopMenu()
        {
            //categories
            var cachedCategoriesModel = PrepareCategorySimpleModels();
            ////top menu topics
            //string topicCacheKey = string.Format(ModelCacheEventConsumer.TOPIC_TOP_MENU_MODEL_KEY,
            //    _workContext.WorkingLanguage.Id,
            //    _storeContext.CurrentStore.Id,
            //    string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()));
            //var cachedTopicModel = _cacheManager.Get(topicCacheKey, () =>
            //    _topicService.GetAllTopics(_storeContext.CurrentStore.Id)
            //    .Where(t => t.IncludeInTopMenu)
            //    .Select(t => new TopMenuModel.TopMenuTopicModel
            //    {
            //        Id = t.Id,
            //        Name = t.GetLocalized(x => x.Title),
            //        SeName = t.GetSeName()
            //    })
            //    .ToList()
            //);

            //string manufacturerCacheKey = string.Format(ModelCacheEventConsumer.MANUFACTURER_NAVIGATION_MENU,
            //    _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id);

            //var cachedManufacturerModel = _cacheManager.Get(manufacturerCacheKey, () =>
            //    _manufacturerService.GetAllManufacturers(storeId: _storeContext.CurrentStore.Id)
            //        .Where(x => x.IncludeInTopMenu)
            //        .Select(t => new TopMenuModel.TopMenuManufacturerModel
            //        {
            //            Id = t.Id,
            //            Name = t.GetLocalized(x => x.Name),
            //            SeName = t.GetSeName()
            //        })
            //        .ToList()
            //    );


            var model = new TopMenuModel
            {
                Categories = cachedCategoriesModel,
                //Topics = cachedTopicModel,
                //Manufacturers = cachedManufacturerModel,
                NewProductsEnabled = _catalogSettings.NewProductsEnabled,
                BlogEnabled = _blogSettings.Enabled,
                ForumEnabled = _forumSettings.ForumsEnabled,
                DisplayHomePageMenu = _menuItemSettings.DisplayHomePageMenu,
                DisplayNewProductsMenu = _menuItemSettings.DisplayNewProductsMenu,
                DisplaySearchMenu = _menuItemSettings.DisplaySearchMenu,
                DisplayCustomerMenu = _menuItemSettings.DisplayCustomerMenu,
                DisplayBlogMenu = _menuItemSettings.DisplayBlogMenu,
                DisplayForumsMenu = _menuItemSettings.DisplayForumsMenu,
                DisplayContactUsMenu = _menuItemSettings.DisplayContactUsMenu
            };

            return model;

        }

        #endregion

        #region Manufacturer

        public virtual string PrepareManufacturerTemplateViewPath(string templateId)
        {
            var templateCacheKey = string.Format(ModelCacheEventConsumer.MANUFACTURER_TEMPLATE_MODEL_KEY, templateId);
            var templateViewPath = _cacheManager.Get(templateCacheKey, () =>
            {
                var template = _manufacturerTemplateService.GetManufacturerTemplateById(templateId);
                if (template == null)
                    template = _manufacturerTemplateService.GetAllManufacturerTemplates().FirstOrDefault();
                if (template == null)
                    throw new Exception("No default template could be loaded");
                return template.ViewPath;
            });
            return templateViewPath;
        }
        public virtual List<ManufacturerModel> PrepareManufacturerAll()
        {
            var model = new List<ManufacturerModel>();
            var manufacturers = _manufacturerService.GetAllManufacturers(storeId: _storeContext.CurrentStore.Id);
            foreach (var manufacturer in manufacturers)
            {
                var modelMan = manufacturer.ToModel();

                //prepare picture model
                int pictureSize = _mediaSettings.ManufacturerThumbPictureSize;
                var manufacturerPictureCacheKey = string.Format(ModelCacheEventConsumer.MANUFACTURER_PICTURE_MODEL_KEY, manufacturer.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
                modelMan.PictureModel = _cacheManager.Get(manufacturerPictureCacheKey, () =>
                {
                    var picture = _pictureService.GetPictureById(manufacturer.PictureId);
                    var pictureModel = new PictureModel
                    {
                        FullSizeImageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.ApplyWatermarkForManufacturer),
                        ImageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.ApplyWatermarkForManufacturer, pictureSize),
                        Title = string.Format(_localizationService.GetResource("Media.Manufacturer.ImageLinkTitleFormat"), modelMan.Name),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Manufacturer.ImageAlternateTextFormat"), modelMan.Name)
                    };
                    return pictureModel;
                });
                model.Add(modelMan);
            }
            return model;
        }
        public virtual List<ManufacturerModel> PrepareHomepageManufacturers()
        {
            string manufacturersCacheKey = string.Format(ModelCacheEventConsumer.MANUFACTURER_HOMEPAGE_KEY,
                _storeContext.CurrentStore.Id,
                _workContext.WorkingLanguage.Id);

            List<ManufacturerModel> model = _cacheManager.Get(manufacturersCacheKey, () =>
                _manufacturerService.GetAllManufacturers(storeId: _storeContext.CurrentStore.Id)
                .Where(x => x.ShowOnHomePage)
                .Select(x =>
                {
                    var manModel = x.ToModel();
                    //prepare picture model
                    int pictureSize = _mediaSettings.CategoryThumbPictureSize;
                    var categoryPictureCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_PICTURE_MODEL_KEY, x.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
                    manModel.PictureModel = _cacheManager.Get(categoryPictureCacheKey, () =>
                    {
                        var picture = _pictureService.GetPictureById(x.PictureId);
                        var pictureModel = new PictureModel
                        {
                            FullSizeImageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.ApplyWatermarkForCategory),
                            ImageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.ApplyWatermarkForCategory, pictureSize),
                            Title = string.Format(_localizationService.GetResource("Media.Manufacturer.ImageLinkTitleFormat"), manModel.Name),
                            AlternateText = string.Format(_localizationService.GetResource("Media.Manufacturer.ImageAlternateTextFormat"), manModel.Name)
                        };
                        return pictureModel;
                    });
                    return manModel;
                })
                .ToList()
            );
            return model;
        }
        public virtual ManufacturerNavigationModel PrepareManufacturerNavigation(string currentManufacturerId)
        {
            string cacheKey = string.Format(ModelCacheEventConsumer.MANUFACTURER_NAVIGATION_MODEL_KEY,
                currentManufacturerId,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () =>
            {
                var currentManufacturer = _manufacturerService.GetManufacturerById(currentManufacturerId);

                var manufacturers = _manufacturerService.GetAllManufacturers(pageSize: _catalogSettings.ManufacturersBlockItemsToDisplay, storeId: _storeContext.CurrentStore.Id);
                var model = new ManufacturerNavigationModel
                {
                    TotalManufacturers = manufacturers.TotalCount
                };

                foreach (var manufacturer in manufacturers)
                {
                    var modelMan = new ManufacturerBriefInfoModel
                    {
                        Id = manufacturer.Id,
                        Name = manufacturer.GetLocalized(x => x.Name),
                        SeName = manufacturer.GetSeName(),
                        IsActive = currentManufacturer != null && currentManufacturer.Id == manufacturer.Id,
                    };
                    model.Manufacturers.Add(modelMan);
                }
                return model;
            });
            return cacheModel;
        }
        public virtual ManufacturerModel PrepareManufacturer(Manufacturer manufacturer, CatalogPagingFilteringModel command)
        {
            var model = manufacturer.ToModel();

            //sorting
            PrepareSortingOptions(model.PagingFilteringContext, command);
            //view mode
            PrepareViewModes(model.PagingFilteringContext, command);
            //page size
            PreparePageSizeOptions(model.PagingFilteringContext, command,
                manufacturer.AllowCustomersToSelectPageSize,
                manufacturer.PageSizeOptions,
                manufacturer.PageSize);


            //price ranges
            model.PagingFilteringContext.PriceRangeFilter.LoadPriceRangeFilters(manufacturer.PriceRanges, _webHelper, _priceFormatter);
            var selectedPriceRange = model.PagingFilteringContext.PriceRangeFilter.GetSelectedPriceRange(_webHelper, manufacturer.PriceRanges);
            decimal? minPriceConverted = null;
            decimal? maxPriceConverted = null;
            if (selectedPriceRange != null)
            {
                if (selectedPriceRange.From.HasValue)
                    minPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(selectedPriceRange.From.Value, _workContext.WorkingCurrency);

                if (selectedPriceRange.To.HasValue)
                    maxPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(selectedPriceRange.To.Value, _workContext.WorkingCurrency);
            }



            //featured products
            if (!_catalogSettings.IgnoreFeaturedProducts)
            {
                IPagedList<Product> featuredProducts = null;

                //We cache a value indicating whether we have featured products
                string cacheKey = string.Format(ModelCacheEventConsumer.MANUFACTURER_HAS_FEATURED_PRODUCTS_KEY,
                    manufacturer.Id,
                    string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                    _storeContext.CurrentStore.Id);
                var hasFeaturedProductsCache = _cacheManager.Get<bool?>(cacheKey);
                if (!hasFeaturedProductsCache.HasValue)
                {
                    //no value in the cache yet
                    //let's load products and cache the result (true/false)
                    featuredProducts = _productService.SearchProducts(
                       pageSize: _catalogSettings.LimitOfFeaturedProducts,
                       manufacturerId: manufacturer.Id,
                       storeId: _storeContext.CurrentStore.Id,
                       visibleIndividuallyOnly: true,
                       featuredProducts: true);
                    hasFeaturedProductsCache = featuredProducts.Any();
                    _cacheManager.Set(cacheKey, hasFeaturedProductsCache, 60);
                }
                if (hasFeaturedProductsCache.Value && featuredProducts == null)
                {
                    //cache indicates that the manufacturer has featured products
                    //let's load them
                    featuredProducts = _productService.SearchProducts(
                       pageSize: _catalogSettings.LimitOfFeaturedProducts,
                       manufacturerId: manufacturer.Id,
                       storeId: _storeContext.CurrentStore.Id,
                       visibleIndividuallyOnly: true,
                       featuredProducts: true);
                }
                if (featuredProducts != null)
                {
                    model.FeaturedProducts = _productWebService.PrepareProductOverviewModels(featuredProducts).ToList();
                }
            }
            //products
            IList<string> filterableSpecificationAttributeOptionIds;
            var products = _productService.SearchProducts(out filterableSpecificationAttributeOptionIds, false,
                manufacturerId: manufacturer.Id,
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true,
                featuredProducts: _catalogSettings.IncludeFeaturedProductsInNormalLists ? null : (bool?)false,
                priceMin: minPriceConverted,
                priceMax: maxPriceConverted,
                orderBy: (ProductSortingEnum)command.OrderBy,
                pageIndex: command.PageNumber - 1,
                pageSize: command.PageSize);
            model.Products = _productWebService.PrepareProductOverviewModels(products).ToList();

            model.PagingFilteringContext.LoadPagedList(products);

            return model;
        }
        #endregion

        #region Vendors

        public virtual VendorModel PrepareVendor(Vendor vendor, CatalogPagingFilteringModel command)
        {
            var model = new VendorModel
            {
                Id = vendor.Id,
                Name = vendor.GetLocalized(x => x.Name),
                Description = vendor.GetLocalized(x => x.Description),
                MetaKeywords = vendor.GetLocalized(x => x.MetaKeywords),
                MetaDescription = vendor.GetLocalized(x => x.MetaDescription),
                MetaTitle = vendor.GetLocalized(x => x.MetaTitle),
                SeName = vendor.GetSeName(),
                AllowCustomersToContactVendors = _vendorSettings.AllowCustomersToContactVendors
            };

            //sorting
            PrepareSortingOptions(model.PagingFilteringContext, command);
            //view mode
            PrepareViewModes(model.PagingFilteringContext, command);
            //page size
            PreparePageSizeOptions(model.PagingFilteringContext, command,
                vendor.AllowCustomersToSelectPageSize,
                vendor.PageSizeOptions,
                vendor.PageSize);

            //products
            IList<string> filterableSpecificationAttributeOptionIds;
            var products = _productService.SearchProducts(out filterableSpecificationAttributeOptionIds, true,
                vendorId: vendor.Id,
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true,
                orderBy: (ProductSortingEnum)command.OrderBy,
                pageIndex: command.PageNumber - 1,
                pageSize: command.PageSize);
            model.Products = _productWebService.PrepareProductOverviewModels(products).ToList();

            model.PagingFilteringContext.LoadPagedList(products);
            return model;
        }
        public virtual List<VendorModel> PrepareVendorAll()
        {
            var model = new List<VendorModel>();
            var vendors = _vendorService.GetAllVendors();
            foreach (var vendor in vendors)
            {
                var vendorModel = new VendorModel
                {
                    Id = vendor.Id,
                    Name = vendor.GetLocalized(x => x.Name),
                    Description = vendor.GetLocalized(x => x.Description),
                    MetaKeywords = vendor.GetLocalized(x => x.MetaKeywords),
                    MetaDescription = vendor.GetLocalized(x => x.MetaDescription),
                    MetaTitle = vendor.GetLocalized(x => x.MetaTitle),
                    SeName = vendor.GetSeName(),
                    AllowCustomersToContactVendors = _vendorSettings.AllowCustomersToContactVendors
                };
                //prepare picture model
                int pictureSize = _mediaSettings.VendorThumbPictureSize;
                var pictureCacheKey = string.Format(ModelCacheEventConsumer.VENDOR_PICTURE_MODEL_KEY, vendor.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
                vendorModel.PictureModel = _cacheManager.Get(pictureCacheKey, () =>
                {
                    var picture = _pictureService.GetPictureById(vendor.PictureId);
                    var pictureModel = new PictureModel
                    {
                        FullSizeImageUrl = _pictureService.GetPictureUrl(picture, false),
                        ImageUrl = _pictureService.GetPictureUrl(picture, false, pictureSize),
                        Title = string.Format(_localizationService.GetResource("Media.Vendor.ImageLinkTitleFormat"), vendorModel.Name),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Vendor.ImageAlternateTextFormat"), vendorModel.Name)
                    };
                    return pictureModel;
                });
                model.Add(vendorModel);
            }

            return model;
        }
        public virtual VendorNavigationModel PrepareVendorNavigation()
        {
            string cacheKey = ModelCacheEventConsumer.VENDOR_NAVIGATION_MODEL_KEY;
            var cacheModel = _cacheManager.Get(cacheKey, () =>
            {
                var vendors = _vendorService.GetAllVendors(pageSize: _vendorSettings.VendorsBlockItemsToDisplay);
                var model = new VendorNavigationModel
                {
                    TotalVendors = vendors.TotalCount
                };

                foreach (var vendor in vendors)
                {
                    model.Vendors.Add(new VendorBriefInfoModel
                    {
                        Id = vendor.Id,
                        Name = vendor.GetLocalized(x => x.Name),
                        SeName = vendor.GetSeName(),
                    });
                }
                return model;
            });
            return cacheModel;
        }

        #endregion

        #region Tags
        public virtual ProductsByTagModel PrepareProductsByTag(ProductTag productTag, CatalogPagingFilteringModel command)
        {
            var model = new ProductsByTagModel
            {
                Id = productTag.Id,
                TagName = productTag.GetLocalized(y => y.Name),
                TagSeName = productTag.GetSeName()
            };


            //sorting
            PrepareSortingOptions(model.PagingFilteringContext, command);
            //view mode
            PrepareViewModes(model.PagingFilteringContext, command);
            //page size
            PreparePageSizeOptions(model.PagingFilteringContext, command,
                _catalogSettings.ProductsByTagAllowCustomersToSelectPageSize,
                _catalogSettings.ProductsByTagPageSizeOptions,
                _catalogSettings.ProductsByTagPageSize);


            //products
            var products = _productService.SearchProducts(
                storeId: _storeContext.CurrentStore.Id,
                productTagId: productTag.Id,
                visibleIndividuallyOnly: true,
                orderBy: (ProductSortingEnum)command.OrderBy,
                pageIndex: command.PageNumber - 1,
                pageSize: command.PageSize);
            model.Products = _productWebService.PrepareProductOverviewModels(products).ToList();

            model.PagingFilteringContext.LoadPagedList(products);

            return model;
        }
        public virtual PopularProductTagsModel PreparePopularProductTags()
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.PRODUCTTAG_POPULAR_MODEL_KEY, _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () =>
            {
                var model = new PopularProductTagsModel();

                //get all tags
                var allTags = _productTagService
                    .GetAllProductTags()
                    .OrderByDescending(x => _productTagService.GetProductCount(x.Id, _storeContext.CurrentStore.Id))
                    .ToList();

                var tags = allTags
                    .Take(_catalogSettings.NumberOfProductTags)
                    .ToList();
                //sorting
                tags = tags.OrderBy(x => x.GetLocalized(y => y.Name)).ToList();

                model.TotalTags = allTags.Count;

                foreach (var tag in tags)
                    model.Tags.Add(new ProductTagModel
                    {
                        Id = tag.Id,
                        Name = tag.GetLocalized(y => y.Name),
                        SeName = tag.GetSeName(),
                        ProductCount = _productTagService.GetProductCount(tag.Id, _storeContext.CurrentStore.Id)
                    });
                return model;
            });
            return cacheModel;
        }
        public virtual PopularProductTagsModel PrepareProductTagsAll()
        {
            var model = new PopularProductTagsModel();
            model.Tags = _productTagService
                .GetAllProductTags()
                .OrderBy(x => x.GetLocalized(y => y.Name))
                .Select(x =>
                {
                    var ptModel = new ProductTagModel
                    {
                        Id = x.Id,
                        Name = x.GetLocalized(y => y.Name),
                        SeName = x.GetSeName(),
                        ProductCount = _productTagService.GetProductCount(x.Id, _storeContext.CurrentStore.Id)
                    };
                    return ptModel;
                })
                .ToList();
            return model;
        }

        #endregion

        #region Search
        public virtual SearchBoxModel PrepareSearchBox()
        {
            var model = new SearchBoxModel
            {
                AutoCompleteEnabled = _catalogSettings.ProductSearchAutoCompleteEnabled,
                ShowProductImagesInSearchAutoComplete = _catalogSettings.ShowProductImagesInSearchAutoComplete,
                SearchTermMinimumLength = _catalogSettings.ProductSearchTermMinimumLength
            };
            return model;
        }

        public virtual SearchModel PrepareSearch(SearchModel model, CatalogPagingFilteringModel command)
        {
            if (model == null)
                model = new SearchModel();

            var searchTerms = model.q;
            if (searchTerms == null)
                searchTerms = "";
            searchTerms = searchTerms.Trim();

            //sorting
            PrepareSortingOptions(model.PagingFilteringContext, command);
            //view mode
            PrepareViewModes(model.PagingFilteringContext, command);
            //page size
            PreparePageSizeOptions(model.PagingFilteringContext, command,
                _catalogSettings.SearchPageAllowCustomersToSelectPageSize,
                _catalogSettings.SearchPagePageSizeOptions,
                _catalogSettings.SearchPageProductsPerPage);


            string cacheKey = string.Format(ModelCacheEventConsumer.SEARCH_CATEGORIES_MODEL_KEY,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            var categories = _cacheManager.Get(cacheKey, () =>
            {
                var categoriesModel = new List<SearchModel.CategoryModel>();
                //all categories
                var allCategories = _categoryService.GetAllCategories(storeId: _storeContext.CurrentStore.Id);
                foreach (var c in allCategories)
                {
                    //generate full category name (breadcrumb)
                    string categoryBreadcrumb = "";
                    var breadcrumb = c.GetCategoryBreadCrumb(allCategories, _aclService, _storeMappingService);
                    for (int i = 0; i <= breadcrumb.Count - 1; i++)
                    {
                        categoryBreadcrumb += breadcrumb[i].GetLocalized(x => x.Name);
                        if (i != breadcrumb.Count - 1)
                            categoryBreadcrumb += " >> ";
                    }
                    categoriesModel.Add(new SearchModel.CategoryModel
                    {
                        Id = c.Id,
                        Breadcrumb = categoryBreadcrumb
                    });
                }
                return categoriesModel;
            });
            if (categories.Any())
            {
                //first empty entry
                model.AvailableCategories.Add(new SelectListItem
                {
                    Value = "",
                    Text = _localizationService.GetResource("Common.All")
                });
                //all other categories
                foreach (var c in categories)
                {
                    model.AvailableCategories.Add(new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Breadcrumb,
                        Selected = model.cid == c.Id
                    });
                }
            }

            var manufacturers = _manufacturerService.GetAllManufacturers();
            if (manufacturers.Any())
            {
                model.AvailableManufacturers.Add(new SelectListItem
                {
                    Value = "",
                    Text = _localizationService.GetResource("Common.All")
                });
                foreach (var m in manufacturers)
                    model.AvailableManufacturers.Add(new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = m.GetLocalized(x => x.Name),
                        Selected = model.mid == m.Id
                    });
            }

            model.asv = _vendorSettings.AllowSearchByVendor;
            if (model.asv)
            {
                var vendors = _vendorService.GetAllVendors();
                if (vendors.Any())
                {
                    model.AvailableVendors.Add(new SelectListItem
                    {
                        Value = "",
                        Text = _localizationService.GetResource("Common.All")
                    });
                    foreach (var vendor in vendors)
                        model.AvailableVendors.Add(new SelectListItem
                        {
                            Value = vendor.Id.ToString(),
                            Text = vendor.GetLocalized(x => x.Name),
                            Selected = model.vid == vendor.Id
                        });
                }
            }

            IPagedList<Product> products = new PagedList<Product>(new List<Product>(), 0, 1);
            // only search if query string search keyword is set (used to avoid searching or displaying search term min length error message on /search page load)
            var isSearchTermSpecified = false;
            try
            {
                //tbh the ll this is
                //isSearchTermSpecified = _httpContextAccessor.HttpContext.Request.  .Params["q"] != null;

                isSearchTermSpecified = false;

            }
            catch
            {
                isSearchTermSpecified = !String.IsNullOrEmpty(searchTerms);
            }
            if (isSearchTermSpecified)
            {
                if (searchTerms.Length < _catalogSettings.ProductSearchTermMinimumLength)
                {
                    model.Warning = string.Format(_localizationService.GetResource("Search.SearchTermMinimumLengthIsNCharacters"), _catalogSettings.ProductSearchTermMinimumLength);
                }
                else
                {
                    var categoryIds = new List<string>();
                    string manufacturerId = "";
                    decimal? minPriceConverted = null;
                    decimal? maxPriceConverted = null;
                    bool searchInDescriptions = false;
                    string vendorId = "";
                    if (model.adv)
                    {
                        //advanced search
                        var categoryId = model.cid;
                        if (!String.IsNullOrEmpty(categoryId))
                        {
                            categoryIds.Add(categoryId);
                            if (model.isc)
                            {
                                //include subcategories
                                categoryIds.AddRange(GetChildCategoryIds(categoryId));
                            }
                        }


                        manufacturerId = model.mid;

                        //min price
                        if (!string.IsNullOrEmpty(model.pf))
                        {
                            decimal minPrice;
                            if (decimal.TryParse(model.pf, out minPrice))
                                minPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(minPrice, _workContext.WorkingCurrency);
                        }
                        //max price
                        if (!string.IsNullOrEmpty(model.pt))
                        {
                            decimal maxPrice;
                            if (decimal.TryParse(model.pt, out maxPrice))
                                maxPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(maxPrice, _workContext.WorkingCurrency);
                        }

                        searchInDescriptions = model.sid;
                        if (model.asv)
                            vendorId = model.vid;
                    }

                    //var searchInProductTags = false;
                    var searchInProductTags = searchInDescriptions;

                    //products
                    products = _productService.SearchProducts(
                        categoryIds: categoryIds,
                        manufacturerId: manufacturerId,
                        storeId: _storeContext.CurrentStore.Id,
                        visibleIndividuallyOnly: true,
                        priceMin: minPriceConverted,
                        priceMax: maxPriceConverted,
                        keywords: searchTerms,
                        searchDescriptions: searchInDescriptions,
                        searchSku: searchInDescriptions,
                        searchProductTags: searchInProductTags,
                        languageId: _workContext.WorkingLanguage.Id,
                        orderBy: (ProductSortingEnum)command.OrderBy,
                        pageIndex: command.PageNumber - 1,
                        pageSize: command.PageSize,
                        vendorId: vendorId);
                    model.Products = _productWebService.PrepareProductOverviewModels(products).ToList();

                    model.NoResults = !model.Products.Any();

                    //search term statistics
                    if (!String.IsNullOrEmpty(searchTerms))
                    {
                        var searchTerm = _searchTermService.GetSearchTermByKeyword(searchTerms, _storeContext.CurrentStore.Id);
                        if (searchTerm != null)
                        {
                            searchTerm.Count++;
                            _searchTermService.UpdateSearchTerm(searchTerm);
                        }
                        else
                        {
                            searchTerm = new SearchTerm
                            {
                                Keyword = searchTerms,
                                StoreId = _storeContext.CurrentStore.Id,
                                Count = 1
                            };
                            _searchTermService.InsertSearchTerm(searchTerm);
                        }
                    }

                    //event
                    _eventPublisher.Publish(new ProductSearchEvent
                    {
                        SearchTerm = searchTerms,
                        SearchInDescriptions = searchInDescriptions,
                        CategoryIds = categoryIds,
                        ManufacturerId = manufacturerId,
                        WorkingLanguageId = _workContext.WorkingLanguage.Id,
                        VendorId = vendorId
                    });
                }
            }

            model.PagingFilteringContext.LoadPagedList(products);

            return model;
        }
        #endregion

    }
}