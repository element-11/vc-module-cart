﻿using System;
using Microsoft.Practices.Unity;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.CartModule.Data.Services;
using VirtoCommerce.CartModule.Data.Workflow;
using VirtoCommerce.Domain.Cart.Model;
using VirtoCommerce.Domain.Cart.Services;
using VirtoCommerce.Domain.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;

namespace VirtoCommerce.CartModule.Web
{
    public class Module : IModule
    {
        private readonly IUnityContainer _container;

        public Module(IUnityContainer container)
        {
            _container = container;
        }

        #region IModule Members

        public void SetupDatabase(SampleDataLevel sampleDataLevel)
        {
            using (var context = new CartRepositoryImpl())
            {
                var initializer = new SetupDatabaseInitializer<CartRepositoryImpl, VirtoCommerce.CartModule.Data.Migrations.Configuration>();
                initializer.InitializeDatabase(context);
            }

        }

        public void Initialize()
        {
            //Business logic for core model
			var cartWorkflowService = new ShoppingCartWorflow();
            //Subscribe to cart changes. Calculate totals  
            cartWorkflowService.Subscribe(new CalculateTotalsActivity());
            _container.RegisterInstance<IShoppingCartWorkflow>(cartWorkflowService);

			_container.RegisterType<ICartRepository>(new InjectionFactory(c => new CartRepositoryImpl("VirtoCommerce", new EntityPrimaryKeyGeneratorInterceptor(), new AuditableInterceptor())));

            _container.RegisterType<IShoppingCartService, ShoppingCartServiceImpl>();
            _container.RegisterType<IShoppingCartSearchService, ShoppingCartSearchServiceImpl>();
        }

        public void PostInitialize()
        {
        }

        #endregion
    }
}