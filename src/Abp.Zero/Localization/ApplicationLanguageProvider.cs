using System.Collections.Generic;
using System.Linq;
using Abp.Configuration;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.Threading;

namespace Abp.Localization
{
    /// <summary>
    /// Implements <see cref="ILanguageProvider"/> to get languages from <see cref="IApplicationLanguageManager"/>.
    /// </summary>
    public class ApplicationLanguageProvider : ILanguageProvider
    {
        /// <summary>
        /// Reference to the session.
        /// </summary>
        public IAbpSession AbpSession { get; set; }

        private readonly IApplicationLanguageManager _applicationLanguageManager;
        private readonly ISettingManager _settingManager;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ApplicationLanguageProvider(
            IApplicationLanguageManager applicationLanguageManager, 
            ISettingManager settingManager)
        {
            _applicationLanguageManager = applicationLanguageManager;
            _settingManager = settingManager;
            AbpSession = NullAbpSession.Instance;
        }

        /// <summary>
        /// Gets the languages for current tenant.
        /// </summary>
        public IReadOnlyList<LanguageInfo> GetLanguages()
        {
            var languageInfos = AsyncHelper.RunSync(() => _applicationLanguageManager.GetLanguagesAsync(AbpSession.TenantId))
                    .OrderBy(l => l.DisplayName)
                    .Select(l => l.ToLanguageInfo())
                    .ToList();

            var defaultLanguage = AsyncHelper.RunSync(() => _applicationLanguageManager.GetDefaultLanguageOrNullAsync(AbpSession.TenantId));
            if (defaultLanguage != null)
            {
                var languageInfo = languageInfos.FirstOrDefault(l => l.Name == defaultLanguage.Name);
                if (languageInfo != null)
                {
                    languageInfo.IsDefault = true;
                }
            }

            return languageInfos;
        }
    }
}