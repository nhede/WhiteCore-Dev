﻿/*
 * Copyright (c) Contributors, http://whitecore-sim.org/, http://aurora-sim.org/
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the Aurora-Sim Project nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */


using WhiteCore.Framework.ConsoleFramework;
using WhiteCore.Framework.Modules;
using Nini.Config;
using OpenMetaverse;
using OpenMetaverse.StructuredData;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace WhiteCore.Modules.Currency
{
    public class SimpleCurrencyConfig : IDataTransferable
    {
        #region declarations

        private uint m_PriceUpload = 0;
        private uint m_PriceGroupCreate = 0;
        private uint m_PriceDirectoryFee = 0;
        private int m_Stipend = 0;
        private string m_UpgradeMembershipUri = "";
        private string m_ErrorURI = "";
        private int m_SchedulerInterval = 0;
        private bool m_CanBuyCurrencyInworld = true;
        private bool m_GiveStipends = false;
        private string m_StipendsEveryType = "month";
        private bool m_StipendsPremiumOnly = false;
        private int m_StipendsEvery = 1;
        private uint m_ClientPort = 8002;
        private bool m_StipendsLoadOldUsers = true;
        private bool m_GiveStipendsOnlyWhenLoggedIn = false;
        private bool m_SaveTransactionLogs = false;
        private int m_MaxAmountBeforeLogging = -1;
        private int m_AdditionPercentage = 291;
        private int m_AdditionAmount = 30;
        private int m_RealCurrencyConversionFactor = 1;
        private int m_MaxAmountPurchasable = 10000;
        private int m_MaxAmountPurchasableOverTime = 100000;
        private int m_MaxAmountPurchasableEveryAmount = 1;
        private string m_MaxAmountPurchasableEveryType = "week";
        private int m_MinAmountPurchasable = 0;

        #endregion

        #region functions

        public SimpleCurrencyConfig(IConfig economyConfig)
        {
            foreach (PropertyInfo propertyInfo in GetType().GetProperties())
            {
                try
                {
                    if (propertyInfo.PropertyType.IsAssignableFrom(typeof (float)))
                        propertyInfo.SetValue(this,
                                              economyConfig.GetFloat(propertyInfo.Name,
                                                                     float.Parse(
                                                                         propertyInfo.GetValue(this, new object[0])
                                                                                     .ToString())), new object[0]);
                    else if (propertyInfo.PropertyType.IsAssignableFrom(typeof (int)))
                        propertyInfo.SetValue(this,
                                              economyConfig.GetInt(propertyInfo.Name,
                                                                   int.Parse(
                                                                       propertyInfo.GetValue(this, new object[0])
                                                                                   .ToString())), new object[0]);
                    else if (propertyInfo.PropertyType.IsAssignableFrom(typeof (bool)))
                        propertyInfo.SetValue(this,
                                              economyConfig.GetBoolean(propertyInfo.Name,
                                                                       bool.Parse(
                                                                           propertyInfo.GetValue(this, new object[0])
                                                                                       .ToString())), new object[0]);
                    else if (propertyInfo.PropertyType.IsAssignableFrom(typeof (string)))
                        propertyInfo.SetValue(this,
                                              economyConfig.GetString(propertyInfo.Name,
                                                                      propertyInfo.GetValue(this, new object[0])
                                                                                  .ToString()), new object[0]);
                    else if (propertyInfo.PropertyType.IsAssignableFrom(typeof (UUID)))
                        propertyInfo.SetValue(this,
                                              new UUID(economyConfig.GetString(propertyInfo.Name,
                                                                               propertyInfo.GetValue(this, new object[0])
                                                                                           .ToString())), new object[0]);
                }
                catch (Exception)
                {
                    MainConsole.Instance.Warn("[SimpleCurrency]: Exception reading economy config: " + propertyInfo.Name);
                }
            }
        }

        public SimpleCurrencyConfig()
        {
        }

        public SimpleCurrencyConfig(OSDMap values)
        {
            FromOSD(values);
        }

        public override OSDMap ToOSD()
        {
            OSDMap returnvalue = new OSDMap();
            foreach (PropertyInfo propertyInfo in GetType().GetProperties())
            {
                try
                {
                    if (propertyInfo.PropertyType.IsAssignableFrom(typeof (float)))
                        returnvalue.Add(propertyInfo.Name, (float) propertyInfo.GetValue(this, new object[0]));
                    else if (propertyInfo.PropertyType.IsAssignableFrom(typeof (int)))
                        returnvalue.Add(propertyInfo.Name, (int) propertyInfo.GetValue(this, new object[0]));
                    else if (propertyInfo.PropertyType.IsAssignableFrom(typeof (bool)))
                        returnvalue.Add(propertyInfo.Name, (bool) propertyInfo.GetValue(this, new object[0]));
                    else if (propertyInfo.PropertyType.IsAssignableFrom(typeof (string)))
                        returnvalue.Add(propertyInfo.Name, (string) propertyInfo.GetValue(this, new object[0]));
                    else if (propertyInfo.PropertyType.IsAssignableFrom(typeof (UUID)))
                        returnvalue.Add(propertyInfo.Name, (UUID) propertyInfo.GetValue(this, new object[0]));
                }
                catch (Exception ex)
                {
                    MainConsole.Instance.Warn("[SimpleCurrency]: Exception toOSD() config: " + ex.ToString());
                }
            }
            return returnvalue;
        }

        public override sealed void FromOSD(OSDMap values)
        {
            foreach (PropertyInfo propertyInfo in GetType().GetProperties())
            {
                if (values.ContainsKey(propertyInfo.Name))
                {
                    try
                    {
                        if (propertyInfo.PropertyType.IsAssignableFrom(typeof (float)))
                            propertyInfo.SetValue(this, float.Parse(values[propertyInfo.Name].AsString()), new object[0]);
                        else if (propertyInfo.PropertyType.IsAssignableFrom(typeof (int)))
                            propertyInfo.SetValue(this, values[propertyInfo.Name].AsInteger(), new object[0]);
                        else if (propertyInfo.PropertyType.IsAssignableFrom(typeof (bool)))
                            propertyInfo.SetValue(this, values[propertyInfo.Name].AsBoolean(), new object[0]);
                        else if (propertyInfo.PropertyType.IsAssignableFrom(typeof (string)))
                            propertyInfo.SetValue(this, values[propertyInfo.Name].AsString(), new object[0]);
                        else if (propertyInfo.PropertyType.IsAssignableFrom(typeof (UUID)))
                            propertyInfo.SetValue(this, values[propertyInfo.Name].AsUUID(), new object[0]);
                    }
                    catch (Exception ex)
                    {
                        MainConsole.Instance.Warn("[SimpleCurrency]: Exception reading fromOSD() config: " +
                                                  ex.ToString());
                    }
                }
            }
        }

        #endregion

        #region properties

        public string ErrorURI
        {
            get { return m_ErrorURI; }
            set { m_ErrorURI = value; }
        }

        public string UpgradeMembershipUri
        {
            get { return m_UpgradeMembershipUri; }
            set { m_UpgradeMembershipUri = value; }
        }

        public int Stipend
        {
            get { return m_Stipend; }
            set { m_Stipend = value; }
        }

        public bool GiveStipends
        {
            get { return m_GiveStipends; }
            set { m_GiveStipends = value; }
        }

        public string StipendsEveryType
        {
            get { return m_StipendsEveryType; }
            set { m_StipendsEveryType = value; }
        }

        public int StipendsEvery
        {
            get { return m_StipendsEvery; }
            set { m_StipendsEvery = value; }
        }

        public int PriceGroupCreate
        {
            get { return (int) m_PriceGroupCreate; }
            set { m_PriceGroupCreate = (uint) value; }
        }

        public int PriceUpload
        {
            get { return (int) m_PriceUpload; }
            set { m_PriceUpload = (uint) value; }
        }

        public int PriceDirectoryFee
        {
            get { return (int)m_PriceDirectoryFee; }
            set { m_PriceDirectoryFee = (uint)value; }
        }

        public bool StipendsPremiumOnly
        {
            get { return m_StipendsPremiumOnly; }
            set { m_StipendsPremiumOnly = value; }
        }

        public int ClientPort
        {
            get { return (int) m_ClientPort; }
            set { m_ClientPort = (uint) value; }
        }

        public bool CanBuyCurrencyInworld
        {
            get { return m_CanBuyCurrencyInworld; }
            set { m_CanBuyCurrencyInworld = value; }
        }

        public bool StipendsLoadOldUsers
        {
            get { return m_StipendsLoadOldUsers; }
            set { m_StipendsLoadOldUsers = value; }
        }

        public bool GiveStipendsOnlyWhenLoggedIn
        {
            get { return m_GiveStipendsOnlyWhenLoggedIn; }
            set { m_GiveStipendsOnlyWhenLoggedIn = value; }
        }

        public bool SaveTransactionLogs
        {
            get { return m_SaveTransactionLogs; }
            set { m_SaveTransactionLogs = value; }
        }

        public int MaxAmountBeforeLogging
        {
            get { return m_MaxAmountBeforeLogging; }
            set { m_MaxAmountBeforeLogging = value; }
        }

        public int AdditionPercentage
        {
            get { return m_AdditionPercentage; }
            set { m_AdditionPercentage = value; }
        }

        public int AdditionAmount
        {
            get { return m_AdditionAmount; }
            set { m_AdditionAmount = value; }
        }

        public int RealCurrencyConversionFactor
        {
            get { return m_RealCurrencyConversionFactor; }
            set { m_RealCurrencyConversionFactor = value; }
        }

        public int MaxAmountPurchasable
        {
            get { return m_MaxAmountPurchasable; }
            set { m_MaxAmountPurchasable = value; }
        }

        public int MaxAmountPurchasableOverTime
        {
            get { return m_MaxAmountPurchasableOverTime; }
            set { m_MaxAmountPurchasableOverTime = value; }
        }

        public int MaxAmountPurchasableEveryAmount
        {
            get { return m_MaxAmountPurchasableEveryAmount; }
            set { m_MaxAmountPurchasableEveryAmount = value; }
        }

        public string MaxAmountPurchasableEveryType
        {
            get { return m_MaxAmountPurchasableEveryType; }
            set { m_MaxAmountPurchasableEveryType = value; }
        }

        public int MinAmountPurchasable
        {
            get { return m_MinAmountPurchasable; }
            set { m_MinAmountPurchasable = value; }
        }
        
        public int SchedulerInterval
        {
        	get { return m_SchedulerInterval; }
        	set { m_SchedulerInterval = value; }
        }

        #endregion
    }

    public class UserCurrency : IDataTransferable
    {
        public UUID PrincipalID;
        public uint Amount;
        public uint LandInUse;
        public uint Tier;
        public bool IsGroup;
        public uint StipendsBalance;

        /// <summary>
        /// </summary>
        /// <param name="osdMap"></param>
        public UserCurrency(OSDMap osdMap)
        {
            if (osdMap != null)
                FromOSD(osdMap);
        }

        public UserCurrency(List<string> queryResults)
        {
            FromArray(queryResults);
        }

        public UserCurrency() { }

        public UserCurrency(UUID agentID, uint balance, uint landuse, uint tier_bal, bool group_toggle, uint stipend_bal)
        {
            PrincipalID = agentID;
            Amount = landuse;
            LandInUse = landuse;
            Tier = tier_bal;
            IsGroup = group_toggle;
            StipendsBalance = stipend_bal;
        }

        /// <summary></summary>
        /// <param name="osdMap"></param>
        public override sealed void FromOSD(OSDMap osdMap)
        {
            UUID.TryParse(osdMap["PrincipalID"].AsString(), out PrincipalID);
            uint.TryParse(osdMap["Amount"].AsString(), out Amount);
            uint.TryParse(osdMap["LandInUse"].AsString(), out LandInUse);
            uint.TryParse(osdMap["Tier"].AsString(), out Tier);
            bool.TryParse(osdMap["IsGroup"].AsString(), out IsGroup);
            uint.TryParse(osdMap["StipendsBalance"].AsString(), out StipendsBalance);
        }

        public bool FromArray(List<string> queryResults)
        {
            return UUID.TryParse(queryResults[0], out PrincipalID) &&
                   uint.TryParse(queryResults[1], out Amount) &&
                   uint.TryParse(queryResults[2], out LandInUse) &&
                   uint.TryParse(queryResults[3], out Tier) &&
                   bool.TryParse(queryResults[4], out IsGroup) &&
                   uint.TryParse(queryResults[5], out StipendsBalance);
        }

        /// <summary></summary>
        /// <returns></returns>
        public override OSDMap ToOSD()
        {
            return
                new OSDMap
                    {
                        {"PrincipalID", PrincipalID},
                        {"Amount", Amount},
                        {"LandInUse", LandInUse},
                        {"Tier", Tier},
                        {"IsGroup", IsGroup},
                        {"StipendsBalance", StipendsBalance}
                    };
        }
    }
}