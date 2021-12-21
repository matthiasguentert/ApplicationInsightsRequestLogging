﻿using Azureblue.ApplicationInsights.RequestLogging.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Azureblue.ApplicationInsights.RequestLogging
{
    public class BodyLoggerMiddleware : IMiddleware
    {
        private readonly IOptions<BodyLoggerOptions> _options;
        private readonly IBodyReader _bodyReader;
        private readonly ITelemetryWriter _telemetryWriter;
        private readonly ISensitiveDataFilter _sensitiveDataFilter;

        public BodyLoggerMiddleware(IOptions<BodyLoggerOptions> options, IBodyReader bodyReader, ITelemetryWriter telemetryWriter, ISensitiveDataFilter sensitiveDataFilter)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _bodyReader = bodyReader ?? throw new ArgumentNullException(nameof(bodyReader));
            _telemetryWriter = telemetryWriter ?? throw new ArgumentNullException(nameof(telemetryWriter));
            _sensitiveDataFilter = sensitiveDataFilter ?? throw new ArgumentNullException(nameof(telemetryWriter));
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var requestBody = string.Empty;
            if (_options.Value.HttpVerbs.Contains(context.Request.Method))
            {
                // Temporarily store request body
                requestBody = await _bodyReader.ReadRequestBodyAsync(context, _options.Value.MaxBytes, _options.Value.Appendix);

                _bodyReader.PrepareResponseBodyReading(context);
            }

            // hand over to the next middleware and wait for the call to return
            await next(context);

            if (_options.Value.HttpVerbs.Contains(context.Request.Method))
            {
                if (_options.Value.HttpCodes.Contains(context.Response.StatusCode))
                {
                    var responseBody = await _bodyReader.ReadResponseBodyAsync(context, _options.Value.MaxBytes, _options.Value.Appendix);

                    _telemetryWriter.Write(context, _options.Value.RequestBodyPropertyKey, _sensitiveDataFilter.RemoveSensitiveData(requestBody));
                    _telemetryWriter.Write(context, _options.Value.ResponseBodyPropertyKey, _sensitiveDataFilter.RemoveSensitiveData(responseBody));
                }
                
                // Copy back so response body is available for the user agent
                // prevent 500 error when Not StatusCode of Interest
                await this._bodyReader.RestoreOriginalResponseBodyStreamAsync(context);
            }
        }
    }
}
