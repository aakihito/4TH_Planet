using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocalSelector : MonoBehaviour
{
    public void ChangeLanguage()
    {
        //StartCoroutine(SetNewLocal(newLanguageID));


        if(LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[0])
        {
            StartCoroutine(SetNewLocal((int)Language.PT));
        }
        else if(LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[1])
        {
            StartCoroutine(SetNewLocal((int)Language.EN));
        }
    }

    private IEnumerator SetNewLocal(int languageId)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[languageId];

        SaveSystem.LocalData.id_local = (Language)languageId;
    }
}
