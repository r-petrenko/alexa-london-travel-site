// Copyright (c) Martin Costello, 2017. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.LondonTravel.Site.Services.Data
{
    using System;
    using System.Net.Http;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Extensions.DependencyInjection;
    using Options;

    /// <summary>
    /// A class containing helper methods for DocumentDB operations. This class cannot be inherited.
    /// </summary>
    internal static class DocumentHelpers
    {
        /// <summary>
        /// The relative URI to create a collection. This field is read-only.
        /// </summary>
        internal static readonly Uri CollectionsUriFragment = new Uri("colls", UriKind.Relative);

        /// <summary>
        /// The relative URI to create a collection. This field is read-only.
        /// </summary>
        internal static readonly Uri DocumentsUriFragment = new Uri("docs", UriKind.Relative);

        /// <summary>
        /// Creates a new instance of an <see cref="IDocumentClient"/> implementation.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to use.</param>
        /// <returns>
        /// The created instance of <see cref="IDocumentClient"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serviceProvider"/> is <see langword="null"/>.
        /// </exception>
        internal static IDocumentClient CreateClient(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var options = serviceProvider.GetRequiredService<UserStoreOptions>();
            var factory = serviceProvider.GetRequiredService<IHttpMessageHandlerFactory>();
            var handler = factory.CreateHandler("Azure-CosmosDB");

            return CreateClient(options, handler);
        }

        /// <summary>
        /// Creates a new instance of an <see cref="IDocumentClient"/> implementation.
        /// </summary>
        /// <param name="options">The <see cref="UserStoreOptions"/> to use.</param>
        /// <param name="handler">The <see cref="HttpMessageHandler"/> to use.</param>
        /// <returns>
        /// The created instance of <see cref="IDocumentClient"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="options"/> or <paramref name="handler"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="options"/> is invalid.
        /// </exception>
        internal static IDocumentClient CreateClient(UserStoreOptions options, HttpMessageHandler handler)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            if (options.ServiceUri == null)
            {
                throw new ArgumentException("No DocumentDB URI is configured.", nameof(options));
            }

            if (!options.ServiceUri.IsAbsoluteUri)
            {
                throw new ArgumentException("The configured DocumentDB URI is as it is not an absolute URI.", nameof(options));
            }

            if (string.IsNullOrEmpty(options.AccessKey))
            {
                throw new ArgumentException("No DocumentDB access key is configured.", nameof(options));
            }

            if (string.IsNullOrEmpty(options.DatabaseName))
            {
                throw new ArgumentException("No DocumentDB database name is configured.", nameof(options));
            }

            if (string.IsNullOrEmpty(options.CollectionName))
            {
                throw new ArgumentException("No DocumentDB collection name is configured.", nameof(options));
            }

            var connectionPolicy = ConnectionPolicy.Default;

            if (options.PreferredLocations?.Count > 0)
            {
                connectionPolicy = new ConnectionPolicy();

                foreach (string location in options.PreferredLocations)
                {
                    connectionPolicy.PreferredLocations.Add(location);
                }
            }

            if (!string.IsNullOrEmpty(options.CurrentLocation))
            {
                connectionPolicy.SetCurrentLocation(options.CurrentLocation);
            }

            connectionPolicy.RequestTimeout = TimeSpan.FromSeconds(30);
            connectionPolicy.UserAgentSuffix = "london-travel";

            return new DocumentClient(options.ServiceUri, options.AccessKey, handler, connectionPolicy);
        }
    }
}
