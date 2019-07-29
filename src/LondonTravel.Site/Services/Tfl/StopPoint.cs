// Copyright (c) Martin Costello, 2017. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.LondonTravel.Site.Services.Tfl
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// A class representing information about a stop point on a line. This class cannot be inherited.
    /// </summary>
    public sealed class StopPoint
    {
        /// <summary>
        /// Gets or sets the Id of the stop point.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the stop point.
        /// </summary>
        [JsonPropertyName("commonName")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the latitude of the stop point.
        /// </summary>
        [JsonPropertyName("lat")]
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude of the stop point.
        /// </summary>
        [JsonPropertyName("lon")]
        public double Longitude { get; set; }
    }
}
