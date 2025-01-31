﻿using OmniSharp.Extensions.JsonRpc;
using PortingAssistantExtensionServer.Models;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using System.Linq;
using System.Collections.Generic;

namespace PortingAssistantExtensionServer.Handlers
{
    [Serial, Method("analyzeSolution")]
    internal interface ISolutionAssessmentHandler : IJsonRpcRequestHandler<AnalyzeSolutionRequest, AnalyzeSolutionResponse> { }
    internal class SolutionAssessmentHandler : ISolutionAssessmentHandler
    {
        private readonly ILogger<ISolutionAssessmentHandler> _logger;
        private readonly AnalysisService _analysisService;
        private readonly ILanguageServerFacade _languageServer;
        public SolutionAssessmentHandler(ILogger<SolutionAssessmentHandler> logger,
            ILanguageServerFacade languageServer,
            AnalysisService analysisService)
        {
            _logger = logger;
            _languageServer = languageServer;
            _analysisService = analysisService;
        }

        public async Task<AnalyzeSolutionResponse> Handle(AnalyzeSolutionRequest request, CancellationToken cancellationToken)
        {
            var solutionAnalysisResult = _analysisService.AssessSolutionAsync(request);
            var diagnostics = await _analysisService.GetDiagnosticsAsync(solutionAnalysisResult);
            
            foreach (var diagnostic in diagnostics)
            {
                IList<Diagnostic> diag = new List<Diagnostic>();
                if (_analysisService._openDocuments.ContainsKey(diagnostic.Key))
                {
                    diag = diagnostic.Value;
                }

                if (!_analysisService._openDocuments.ContainsKey(diagnostic.Key) && diagnostic.Value.Count != 0)
                {
                    diag.Add(diagnostic.Value.FirstOrDefault());
                }

                _languageServer.TextDocument.PublishDiagnostics(new PublishDiagnosticsParams()
                {
                    Diagnostics = new Container<Diagnostic>(diag),
                    Uri = diagnostic.Key,
                });
            }

            return new AnalyzeSolutionResponse()
            {
                incompatibleAPis = 1,
                incompatiblePacakges = 1
            };
        }
    }
}
