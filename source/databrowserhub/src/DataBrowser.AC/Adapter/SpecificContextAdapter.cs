using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using DataBrowser.AuthenticationAuthorization;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace DataBrowser.AC.Adapter
{
    public class SpecificContextAdapter : IRequestContext
    {
        private readonly ILogger<SpecificContextAdapter> _logger;
        private readonly RequestContextAdapter _requestContext;

        private List<string> _applicationLangs;

        private string _cacheControl;

        private int? _dashboardId;

        private bool? _ignoreCache;

        private bool? _isCacheRefresh;


        private ClaimsPrincipal _loggedUser;

        private int? _loggedUserId;

        private string _nodeCode;

        private int? _nodeId;

        private int? _templateId;

        private string _userGuid;

        private string _userLang;


        private string _userOperationGuid;

        private int? _viewId;

        public SpecificContextAdapter(ILogger<SpecificContextAdapter> logger, IHttpContextAccessor accessor,
            ILoggerFactory loggerFactory)
        {
            _logger = logger;
            _requestContext = new RequestContextAdapter(loggerFactory.CreateLogger<RequestContextAdapter>(), accessor);
        }

        public string UserGuid
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_userGuid)) return _requestContext.UserGuid;
                return _userGuid;
            }
        }

        public void OverwriteUserGuid(string userGuid)
        {
            _userGuid = userGuid;
        }

        public string UserGuidFromCurrentContext => _requestContext.UserGuid;

        public string UserOperationGuid
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_userOperationGuid)) return _requestContext.UserOperationGuid;
                return _userOperationGuid;
            }
        }

        public void OverwriteUserOperationGuid(string userOperationGuid)
        {
            _userOperationGuid = userOperationGuid;
        }

        public string UserOperationGuidFromCurrentContext => _requestContext.UserOperationGuid;

        public string UserLang
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_userLang)) return _requestContext.UserLang;
                return _userLang;
            }
        }

        public void OverwriteUserLang(string userLang)
        {
            _userLang = userLang?.ToUpperInvariant();
        }

        public string UserLangFromCurrentContext => _requestContext.UserLang;

        public string NodeCode
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_nodeCode)) return _requestContext.NodeCode;
                return _nodeCode;
            }
        }

        public void OverwriteNodeCode(string nodeCode)
        {
            _nodeCode = nodeCode;
        }

        public string NodeCodeFromCurrentContext => _requestContext.NodeCode;

        public int NodeId
        {
            get
            {
                if (!_nodeId.HasValue) return _requestContext.NodeId;
                return _nodeId.Value;
            }
        }

        public void OverwriteNodeId(int? nodeId)
        {
            _nodeId = nodeId;
        }

        public int NodeIdFromCurrentContext => _requestContext.NodeId;

        public int ViewId
        {
            get
            {
                if (!_viewId.HasValue) return _requestContext.ViewId;
                return _viewId.Value;
            }
        }

        public void OverwriteViewId(int? viewId)
        {
            _viewId = viewId;
        }

        public int ViewIdFromCurrentContext => _requestContext.ViewId;

        public int DashboardId
        {
            get
            {
                if (!_dashboardId.HasValue) return _requestContext.DashboardId;
                return _dashboardId.Value;
            }
        }

        public void OverwriteDashboardId(int? dashboardId)
        {
            _dashboardId = dashboardId;
        }

        public int DashboardIdFromCurrentContext => _requestContext.DashboardId;

        public int TemplateId
        {
            get
            {
                if (!_templateId.HasValue) return _requestContext.TemplateId;
                return _templateId.Value;
            }
        }

        public void OverwriteTemplateId(int? templateId)
        {
            _templateId = templateId;
        }

        public int TemplateIdFromCurrentContext => _requestContext.TemplateId;

        public int LoggedUserId
        {
            get
            {
                if (!_loggedUserId.HasValue) return _requestContext.LoggedUserId;
                return _loggedUserId.Value;
            }
        }

        public void OverwriteLoggedUserId(int? loggedUserId)
        {
            _loggedUserId = loggedUserId;
        }

        public int LoggedUserIdFromCurrentContext => _requestContext.LoggedUserId;

        public bool IgnoreCache
        {
            get
            {
                if (!_ignoreCache.HasValue) return _requestContext.IgnoreCache;
                return _ignoreCache.Value;
            }
        }

        public void OverwriteIgnoreCache(bool? ignoreCache)
        {
            _ignoreCache = ignoreCache;
        }

        public bool IgnoreCacheFromCurrentContext => _requestContext.IgnoreCache;

        public List<string> ApplicationLangs
        {
            get
            {
                if (_applicationLangs == null) return _requestContext.ApplicationLangs;
                return _applicationLangs;
            }
        }

        public void OverwriteApplicationLangs(List<string> applicationLangs)
        {
            _applicationLangs = applicationLangs;
        }

        public List<string> ApplicationLangsFromCurrentContext => _requestContext.ApplicationLangs;

        public string CacheControl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_cacheControl)) return _requestContext.CacheControl;
                return _cacheControl;
            }
        }

        public void OverwriteCacheControl(string cacheControl)
        {
            _cacheControl = cacheControl;
        }

        public string CacheControlFromCurrentContext => _requestContext.CacheControl;

        public bool IsCacheRefresh
        {
            get
            {
                if (!_isCacheRefresh.HasValue) return _requestContext.IsCacheRefresh;
                return _isCacheRefresh.Value;
            }
        }

        public void OverwriteIsCacheRefresh(bool? isCacheRefresh)
        {
            _isCacheRefresh = isCacheRefresh;
        }

        public bool IsCacheRefreshFromCurrentContext => _requestContext.IsCacheRefresh;

        public ClaimsPrincipal LoggedUser
        {
            get
            {
                if (!_isCacheRefresh.HasValue) return _requestContext.LoggedUser;
                return _loggedUser;
            }
        }

        public void OverwriteLoggedUser(ClaimsPrincipal loggedUser)
        {
            _loggedUser = loggedUser;
        }

        public ClaimsPrincipal LoggedUserFromCurrentContext => _requestContext.LoggedUser;

        internal class RequestContextAdapter
        {
            private readonly IHttpContextAccessor _accessor;

            private readonly ILogger<RequestContextAdapter> _logger;
            private string _cacheControl;

            private int _dashboardId = -1;


            private bool? _ignoreCache;

            private string _nodeCode;

            private int _nodeId = -1;

            private Guid _operationGuid = Guid.Empty;

            private int _templateId = -1;

            private string _userGuid;

            private string _userLang;

            private int _viewId = -1;

            private bool checkCacheControl;

            public RequestContextAdapter(ILogger<RequestContextAdapter> logger, IHttpContextAccessor accessor)
            {
                _logger = logger;
                _accessor = accessor;
            }

            public string UserGuid
            {
                get
                {
                    if (_userGuid == null && _accessor?.HttpContext?.Request?.Headers != null)
                    {
                        _userGuid = _accessor.HttpContext.Request.Headers[HeaderConstants.Header_UserGuid]
                            .FirstOrDefault();
                        if (_userGuid == null) _userGuid = "";
                    }

                    _logger.LogDebug($"Value UserGuid from Header is {_userGuid}");
                    return _userGuid;
                }
            }

            public string UserLang
            {
                get
                {
                    if (_userLang == null && _accessor?.HttpContext?.Request?.Headers != null)
                    {
                        _userLang = _accessor.HttpContext.Request.Headers[HeaderConstants.Header_UserLang]
                            .FirstOrDefault();
                        _logger.LogDebug($"Value UserLang from Header is {_userLang}");
                        _userLang = _userLang?.ToUpperInvariant() ?? "EN";
                    }

                    if (_userLang != null && "gb".Equals(_userLang, StringComparison.InvariantCultureIgnoreCase))
                        _userLang = "EN";
                    return _userLang ?? "EN";
                }
            }

            public string NodeCode
            {
                get
                {
                    _logger.LogDebug("Read NodeCode");
                    if (_nodeCode == null)
                    {
                        _logger.LogDebug("Generate NodeCode from Header or Claims");
                        var userIdentity = _accessor?.HttpContext?.User?.Identity;
                        if (userIdentity != null && userIdentity.IsAuthenticated)
                        {
                            var clamNodeId = _accessor.HttpContext.User?.Claims?.FirstOrDefault(c =>
                                c.Type != null && c.Type.Equals(HeaderConstants.Claim_NodeCode,
                                    StringComparison.InvariantCultureIgnoreCase));
                            if (clamNodeId != null)
                            {
                                _logger.LogInformation("NodeCode from Claims");
                                _nodeCode = clamNodeId.Value;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(_nodeCode))
                        {
                            //Take from Header only if user not autenticated or haven't nodeId in claims
                            _logger.LogInformation("NodeCode from Headers");
                            if (_accessor?.HttpContext?.Request?.Headers != null)
                                _nodeCode = _accessor.HttpContext.Request.Headers[HeaderConstants.Header_NodeCode]
                                    .FirstOrDefault();
                        }

                        if (string.IsNullOrWhiteSpace(_nodeCode))
                        {
                            //Claim and Header null
                            _logger.LogDebug("Null NodeCode for current request");
                            _nodeCode = "";
                        }
                    }

                    return _nodeCode;
                }
            }

            public int LoggedUserId => UtilitySecurity.GetUserId(_accessor?.HttpContext?.User);

            public ClaimsPrincipal LoggedUser => _accessor?.HttpContext?.User;

            public int NodeId
            {
                get
                {
                    const string name = "nodes";
                    if (_nodeId != -1) return _nodeId;
                    var pathStr = _accessor?.HttpContext?.Request?.Path.ToString().ToLowerInvariant();
                    if (pathStr != null && pathStr.Contains($"/{name}/"))
                    {
                        var splitPath = pathStr.Split('/');
                        var positionNodes = Array.IndexOf(splitPath, name);
                        if (positionNodes > -1 && positionNodes + 1 < splitPath.Length)
                            int.TryParse(splitPath[positionNodes + 1], out _nodeId);
                    }

                    if (_nodeId <= 0) _logger.LogWarning("Node id is undefined");
                    return _nodeId;
                }
            }

            public int ViewId
            {
                get
                {
                    const string name = "views";
                    if (_viewId != -1) return _viewId;
                    var pathStr = _accessor?.HttpContext?.Request?.Path.ToString().ToLowerInvariant();
                    if (pathStr != null && pathStr.Contains($"/{name}/"))
                    {
                        var splitPath = pathStr.Split('/');
                        var positionViews = Array.IndexOf(splitPath, name);
                        if (positionViews > -1 && positionViews + 1 < splitPath.Length)
                            int.TryParse(splitPath[positionViews + 1], out _viewId);
                    }

                    return _viewId;
                }
            }

            public int TemplateId
            {
                get
                {
                    const string name = "templates";
                    if (_templateId != -1) return _templateId;
                    var pathStr = _accessor?.HttpContext?.Request?.Path.ToString().ToLowerInvariant();
                    if (pathStr != null && pathStr.Contains($"/{name}/"))
                    {
                        var splitPath = pathStr.Split('/');
                        var positionViews = Array.IndexOf(splitPath, name);
                        if (positionViews > -1 && positionViews + 1 < splitPath.Length)
                            int.TryParse(splitPath[positionViews + 1], out _templateId);
                    }

                    return _templateId;
                }
            }

            public int DashboardId
            {
                get
                {
                    const string name = "dashboards";
                    if (_dashboardId != -1) return _dashboardId;
                    var pathStr = _accessor?.HttpContext?.Request?.Path.ToString().ToLowerInvariant();
                    if (pathStr != null && pathStr.Contains($"/{name}/"))
                    {
                        var splitPath = pathStr.Split('/');
                        var positionViews = Array.IndexOf(splitPath, name);
                        if (positionViews > -1 && positionViews + 1 < splitPath.Length)
                            int.TryParse(splitPath[positionViews + 1], out _dashboardId);
                    }

                    return _dashboardId;
                }
            }

            public bool IgnoreCache
            {
                get
                {
                    if (_ignoreCache.HasValue) return _ignoreCache.Value;

                    StringValues cacheControl;
                    _accessor?.HttpContext?.Request?.Headers?.TryGetValue("Cache-Control", out cacheControl);
                    if (!StringValues.IsNullOrEmpty(cacheControl) &&
                        (cacheControl.ToString().Equals("no-store", StringComparison.InvariantCultureIgnoreCase) ||
                         cacheControl.ToString().Equals("no-cache", StringComparison.InvariantCultureIgnoreCase)))
                        _ignoreCache = true;
                    else
                        _ignoreCache = false;

                    return _ignoreCache.Value;
                }
            }

            public List<string> ApplicationLangs { get; } = new List<string> {"IT", "EN"};

            public string UserOperationGuid
            {
                get
                {
                    if (_operationGuid == Guid.Empty) _operationGuid = Guid.NewGuid();
                    return _operationGuid.ToString();
                }
            }

            public string CacheControl
            {
                get
                {
                    if (!checkCacheControl)
                    {
                        checkCacheControl = true;
                        if (_accessor?.HttpContext?.Request?.Headers != null &&
                            _accessor.HttpContext.Request.Headers.ContainsKey("Cache-Control"))
                        {
                            StringValues cacheControl;
                            _accessor.HttpContext.Request.Headers.TryGetValue("Cache-Control", out cacheControl);
                            if (!StringValues.IsNullOrEmpty(cacheControl)) _cacheControl = cacheControl.ToString();
                        }
                        else
                        {
                            _cacheControl = null;
                        }
                    }

                    return _cacheControl;
                }
            }

            public bool IsCacheRefresh => !string.IsNullOrWhiteSpace(CacheControl) &&
                                          CacheControl.Equals("refresh", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}