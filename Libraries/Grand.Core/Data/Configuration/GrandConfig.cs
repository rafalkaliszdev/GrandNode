﻿//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Grand.Core.Configuration
//{
//    class GrandConfig
//    {
//    }
//}

using System;
using System.Configuration;
using System.Xml;

namespace Grand.Core.Configuration
{
    /// <summary>
    /// Represents a GrandConfig
    /// </summary>
    public partial class GrandConfig : IConfigurationSectionHandler
    {
        /// <summary>
        /// Creates a configuration section handler.
        /// </summary>
        /// <param name="parent">Parent object.</param>
        /// <param name="configContext">Configuration context object.</param>
        /// <param name="section">Section XML node.</param>
        /// <returns>The created section handler object.</returns>
        public object Create(object parent, object configContext,  /*XmlNode */object section)
        {
            var config = new GrandConfig();

            var section02 = section as XmlNode;
            //ish SelectSingleNode no longer supported in System.Xml
            //var startupNode = section02.SelectSingleNode("Startup");
            //config.IgnoreStartupTasks = GetBool(startupNode, "IgnoreStartupTasks");

            //var redisCachingNode = section02.SelectSingleNode("RedisCaching");
            //config.RedisCachingEnabled = GetBool(redisCachingNode, "Enabled");
            //config.RedisCachingConnectionString = GetString(redisCachingNode, "ConnectionString");

            //var userAgentStringsNode = section02.SelectSingleNode("UserAgentStrings");
            //config.UserAgentStringsPath = GetString(userAgentStringsNode, "databasePath");
            //config.CrawlerOnlyUserAgentStringsPath = GetString(userAgentStringsNode, "crawlersOnlyDatabasePath");

            //var webFarmsNode = section02.SelectSingleNode("WebFarms");
            //config.MultipleInstancesEnabled = GetBool(webFarmsNode, "MultipleInstancesEnabled");
            //config.RunOnAzureWebApps = GetBool(webFarmsNode, "RunOnAzureWebApps");

            //var azureBlobStorageNode = section02.SelectSingleNode("AzureBlobStorage");
            //config.AzureBlobStorageConnectionString = GetString(azureBlobStorageNode, "ConnectionString");
            //config.AzureBlobStorageContainerName = GetString(azureBlobStorageNode, "ContainerName");
            //config.AzureBlobStorageEndPoint = GetString(azureBlobStorageNode, "EndPoint");

            //var amazonS3 = section02.SelectSingleNode("AmazonS3");
            //config.AmazonAwsAccessKeyId = GetString(amazonS3, "AwsAccessKeyId");
            //config.AmazonAwsSecretAccessKey = GetString(amazonS3, "AwsSecretAccessKey");
            //config.AmazonBucketName = GetString(amazonS3, "BucketName");
            //config.AmazonRegion = GetString(amazonS3, "Region");

            //var installationNode = section02.SelectSingleNode("Installation");
            //config.DisableSampleDataDuringInstallation = GetBool(installationNode, "DisableSampleDataDuringInstallation");
            //config.UseFastInstallationService = GetBool(installationNode, "UseFastInstallationService");
            //config.PluginsIgnoredDuringInstallation = GetString(installationNode, "PluginsIgnoredDuringInstallation");

            return config;
        }

        /// <summary>
        /// Indicates whether we should ignore startup tasks
        /// </summary>
        public bool IgnoreStartupTasks { get; private set; }

        /// <summary>
        /// Path to database with user agent strings
        /// </summary>
        public string UserAgentStringsPath { get; private set; }

        public string CrawlerOnlyUserAgentStringsPath { get; private set; }
        /// <summary>
        /// Indicates whether we should use Redis server for caching (instead of default in-memory caching)
        /// </summary>
        public bool RedisCachingEnabled { get; private set; }
        /// <summary>
        /// Redis connection string. Used when Redis caching is enabled
        /// </summary>
        public string RedisCachingConnectionString { get; private set; }


        /// <summary>
        /// A value indicating whether the site is run on multiple instances (e.g. web farm, Windows Azure with multiple instances, etc).
        /// Do not enable it if you run on Azure but use one instance only
        /// </summary>
        public bool MultipleInstancesEnabled { get; private set; }

        /// <summary>
        /// A value indicating whether the site is run on Windows Azure Web Apps
        /// </summary>
        public bool RunOnAzureWebApps { get; private set; }

        /// <summary>
        /// Connection string for Azure BLOB storage
        /// </summary>
        public string AzureBlobStorageConnectionString { get; private set; }

        /// <summary>
        /// Container name for Azure BLOB storage
        /// </summary>
        public string AzureBlobStorageContainerName { get; private set; }
        /// <summary>
        /// End point for Azure BLOB storage
        /// </summary>
        public string AzureBlobStorageEndPoint { get; private set; }

        /// <summary>
        /// Amazon Access Key
        /// </summary>
        public string AmazonAwsAccessKeyId { get; private set; }

        /// <summary>
        /// Amazon Secret Access Key
        /// </summary>
        public string AmazonAwsSecretAccessKey { get; private set; }

        /// <summary>
        /// Amazon Bucket Name using for identifying resources
        /// </summary>
        public string AmazonBucketName { get; private set; }

        /// <summary>
        /// Amazon Region 
        /// http://docs.amazonwebservices.com/AmazonS3/latest/BucketConfiguration.html#LocationSelection
        /// </summary>
        public string AmazonRegion { get; private set; }

        /// <summary>
        /// A value indicating whether a store owner can install sample data during installation
        /// </summary>
        public bool DisableSampleDataDuringInstallation { get; private set; }

        /// <summary>
        /// By default this setting should always be set to "False" (only for advanced users)
        /// </summary>
        public bool UseFastInstallationService { get; private set; }

        /// <summary>
        /// A list of plugins ignored during installation
        /// </summary>
        public string PluginsIgnoredDuringInstallation { get; private set; }

        private string GetString(XmlNode node, string attrName)
        {
            return SetByXElement<string>(node, attrName, Convert.ToString);
        }
        private bool GetBool(XmlNode node, string attrName)
        {
            return SetByXElement<bool>(node, attrName, Convert.ToBoolean);
        }

        private T SetByXElement<T>(XmlNode node, string attrName, Func<string, T> converter)
        {
            if (node == null || node.Attributes == null) return default(T);
            var attr = node.Attributes[attrName];
            if (attr == null) return default(T);
            var attrVal = attr.Value;
            return converter(attrVal);
        }

    }
}
