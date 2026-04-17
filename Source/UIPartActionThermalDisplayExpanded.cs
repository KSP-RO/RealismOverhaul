using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RealismOverhaul
{
    public class UIPartActionThermalDisplayExpanded : UIPartActionThermalDisplay
    {
        internal TextMeshProUGUI txtSkinExposedAreaFrac;
        internal TextMeshProUGUI txtEmissiveConstant;
        internal TextMeshProUGUI txtAbsorptiveConstant;
        internal TextMeshProUGUI txtSkinSkinConduction;
        internal TextMeshProUGUI txtSunFlux;
        internal TextMeshProUGUI txtBodyEmissive;
        internal TextMeshProUGUI txtBodyAlbedo;
        internal TextMeshProUGUI txtRadiationFluxRow;

        private FlightIntegrator _fi;

        internal void Initialize(UIPartActionThermalDisplay source)
        {
            txtThermalMass = source.txtThermalMass;
            txtSkinThermalMass = source.txtSkinThermalMass;
            txtTemperature = source.txtTemperature;
            txtTemperatureExternal = source.txtTemperatureExternal;
            txtConductionInternal = source.txtConductionInternal;
            txtConductionExternal = source.txtConductionExternal;
            txtRadiationExternal = source.txtRadiationExternal;
            txtGeneration = source.txtGeneration;
            txtSkinToInternal = source.txtSkinToInternal;

            if (pawGroup == null)
                pawGroup = source.pawGroup;
        }

        internal void Start()
        {
            RectTransform thermalMassRT = txtThermalMass?.rectTransform;
            RectTransform conductionRT = txtConductionInternal?.rectTransform;
            RectTransform radiationRT = txtRadiationExternal?.rectTransform;
            RectTransform templateRT = txtSkinToInternal?.rectTransform;
            if (thermalMassRT == null || conductionRT == null || radiationRT == null || templateRT == null)
                return;

            Transform rowParent = templateRT.parent;
            float rowStep = GetRowStep(rowParent, templateRT);

            // Capture original Y positions before any shifting.
            float topInsertY = thermalMassRT.anchoredPosition.y;
            float condInsertY = conductionRT.anchoredPosition.y;
            float radInsertY = radiationRT.anchoredPosition.y;

            bool showAbsorptive = part != null && part.absorptiveConstant != part.emissiveConstant;
            int topNewRows = showAbsorptive ? 3 : 2; // skinExpFrac + emissive [+ absorptive]
            const int condNewRows = 1; // after txtConductionInternal
            const int radNewRows = 4;  // after txtRadiationExternal
            int totalNewRows = topNewRows + condNewRows + radNewRows;

            // Each existing row accumulates shifts from every insertion point above it.
            // Rows above txtThermalMass are left untouched.
            for (int i = 0; i < rowParent.childCount; i++)
            {
                var rt = rowParent.GetChild(i) as RectTransform;
                if (rt == null) continue;

                float y = rt.anchoredPosition.y;
                bool atOrBelowTop = rowStep < 0f ? y <= topInsertY : y >= topInsertY;
                if (!atOrBelowTop) continue;

                float shift = rowStep * topNewRows;
                if ((rowStep < 0f && y < condInsertY) || (rowStep > 0f && y > condInsertY))
                    shift += rowStep * condNewRows;
                if ((rowStep < 0f && y < radInsertY) || (rowStep > 0f && y > radInsertY))
                    shift += rowStep * radNewRows;

                rt.anchoredPosition += new Vector2(0f, shift);
            }

            const float indentX = 10f;

            // Insert top rows at the original txtThermalMass position.
            txtSkinExposedAreaFrac = AddRow(templateRT, rowParent, topInsertY);
            txtEmissiveConstant = AddRow(templateRT, rowParent, topInsertY + rowStep);
            if (showAbsorptive)
                txtAbsorptiveConstant = AddRow(templateRT, rowParent, topInsertY + rowStep * 2f);

            // Insert skin-skin conduction row after txtConductionInternal's new position.
            float newCondY = condInsertY + rowStep * topNewRows;
            txtSkinSkinConduction = AddRow(templateRT, rowParent, newCondY + rowStep, 0);

            // Insert 4 indented sub-rows after txtRadiationExternal's new position.
            float newRadY = radInsertY + rowStep * (topNewRows + condNewRows);
            txtSunFlux = AddRow(templateRT, rowParent, newRadY + rowStep, indentX);
            txtBodyEmissive = AddRow(templateRT, rowParent, newRadY + rowStep * 2f, indentX);
            txtBodyAlbedo = AddRow(templateRT, rowParent, newRadY + rowStep * 3f, indentX);
            txtRadiationFluxRow = AddRow(templateRT, rowParent, newRadY + rowStep * 4f, indentX);

            float expandBy = Mathf.Abs(rowStep) * totalNewRows;
            RectTransform myRT = GetComponent<RectTransform>();

            // Expand every RT from the row container up to our own root and disable
            // any ContentSizeFitter along the way.
            for (RectTransform rt = rowParent.GetComponent<RectTransform>();
                 rt != null;
                 rt = rt == myRT ? null : rt.parent?.GetComponent<RectTransform>())
            {
                ContentSizeFitter csf = rt.GetComponent<ContentSizeFitter>();
                if (csf != null)
                    csf.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

                rt.sizeDelta += new Vector2(0f, expandBy);
            }

            LayoutElement myLE = GetComponent<LayoutElement>();
            if (myLE != null)
                myLE.preferredHeight += expandBy;

            Canvas.ForceUpdateCanvases();
        }

        public override void UpdateItem()
        {
            base.UpdateItem();

            PartThermalData ptd = part?.ptd;
            if (ptd == null)
                return;

            if (txtSkinExposedAreaFrac != null)
                txtSkinExposedAreaFrac.text = $"Skin Exposed Fraction: {part.skinExposedAreaFrac:F2}";
            if (txtEmissiveConstant != null)
                txtEmissiveConstant.text = $"Emissive Constant: {part.emissiveConstant:F2}";
            if (txtAbsorptiveConstant != null)
                txtAbsorptiveConstant.text = $"Absorptive Constant: {part.absorptiveConstant:F2}";

            if (txtSkinSkinConduction != null)
                txtSkinSkinConduction.text = $"Skin-Skin Flux: {ptd.skinSkinConductionFlux * PhysicsGlobals.ThermalConvergenceFactor:F2} kW";

            if (_fi == null)
                _fi = part.vessel?.gameObject.GetComponent<FlightIntegrator>();

            double sunArea = _fi != null ? _fi.GetSunArea(ptd) : 1.0;
            double bodyArea = _fi != null ? _fi.GetBodyArea(ptd) : 1.0;

            double sunKW = ptd.sunFlux * sunArea;
            if (txtSunFlux != null)
                txtSunFlux.text = $"Sun Flux: {sunKW:F2} kW";

            // Split ptd.bodyFlux (already scaled by absorbScalar and densityThermalLerp)
            // into emissive and albedo fractions using the raw FlightIntegrator values.
            double rawEmissive = _fi?.bodyEmissiveFlux ?? 0.0;
            double rawAlbedo = _fi?.bodyAlbedoFlux ?? 0.0;
            double rawTotal = rawEmissive + rawAlbedo;
            double bodyKW = ptd.bodyFlux * bodyArea;

            if (txtBodyEmissive != null)
            {
                double emissiveKW = rawTotal > 0.0 ? bodyKW * rawEmissive / rawTotal : 0.0;
                txtBodyEmissive.text = $"Body Emit Flux: {emissiveKW:F2} kW";
            }

            if (txtBodyAlbedo != null)
            {
                double albedoKW = rawTotal > 0.0 ? bodyKW * rawAlbedo / rawTotal : 0.0;
                txtBodyAlbedo.text = $"Body Albedo Flux: {albedoKW:F2} kW";
            }

            // Emission = absorbed − net radiation flux.
            // thermalRadiationFlux = sunAbsorbed + bodyAbsorbed − emitted
            if (txtRadiationFluxRow != null)
            {
                double emissionKW = sunKW + bodyKW - part.thermalRadiationFlux;
                txtRadiationFluxRow.text = $"Emission Flux: {-emissionKW:F2} kW";
            }
        }

        /// <summary>
        /// Derive the signed Y step between consecutive rows.
        /// </summary>
        /// <param name="rowParent"></param>
        /// <param name="templateRT"></param>
        /// <returns></returns>
        private static float GetRowStep(Transform rowParent, RectTransform templateRT)
        {
            if (rowParent.childCount >= 2)
            {
                var rt0 = rowParent.GetChild(0) as RectTransform;
                var rt1 = rowParent.GetChild(1) as RectTransform;
                if (rt0 != null && rt1 != null)
                {
                    float delta = rt1.anchoredPosition.y - rt0.anchoredPosition.y;
                    if (delta != 0f)
                        return delta;
                }
            }
            return -Mathf.Max(templateRT.rect.height, 1f);
        }

        /// <summary>
        /// Clone a row, place it at the given anchoredPosition.y, and apply an x indent.
        /// </summary>
        /// <param name="templateRT"></param>
        /// <param name="parent"></param>
        /// <param name="anchoredY"></param>
        /// <param name="xOffset"></param>
        /// <returns></returns>
        private static TextMeshProUGUI AddRow(RectTransform templateRT, Transform parent, float anchoredY, float xOffset = 0f)
        {
            GameObject go = Instantiate(templateRT.gameObject, parent);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(templateRT.anchoredPosition.x + xOffset, anchoredY);
            return go.GetComponent<TextMeshProUGUI>() ?? go.GetComponentInChildren<TextMeshProUGUI>();
        }
    }
}
