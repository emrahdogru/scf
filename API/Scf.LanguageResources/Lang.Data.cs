using Scf.LanguageResources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;

namespace Scf
{
    public partial class Lang
    {
        #region FieldNames
        public Lc FirstName = new(
            tr: "Ad",
            en: "Name"
            );

        public Lc LastName = new(
            tr: "Soyad",
            en: "Surname"
            );

        public Lc Email = new(
            tr: "E-posta",
            en: "E-mail"
            );

        public Lc Password = new(
            tr: "Parola",
            en: "Password"
            );

        public Lc ConfirmPassword = new(
            tr: "Doğrulama parolası",
            en: "Confirm Password"
            );

        public Lc PositionName = new(
            tr: "Pozisyon adı",
            en: "Position name"
        );

        public Lc GroupName = new(
            tr: "Grup adı",
            en: "Group name"
        );

        public Lc ExternalId = new(
            tr: "Sicil No",
            en: "External Id"
        );

        public Lc EmployeeTitle = new(
            tr: "Unvan",
            en: "Title"
        );

        public Lc ValidationKey = new (
            tr: "Doğrulama anahtarı",
            en: "Validation key"
        );

        public Lc Language = new(
            tr: "Dil",
            en: "Language"
        );

        public Lc TenantCode = new(
            tr: "Hesap kodu",
            en: "Tenant code"
        );

        public Lc TenantTitle = new(
            tr: "Hesap ûnvanı",
            en: "Tenant title"
        );
        #endregion

        #region EnumTexts
        public Lc PremiumState_Draft = new(
            tr: "Taslak",
            en: "Draft"
            );

        public Lc PremiumState_Distributing = new(
            tr: "Dağıtımda",
            en: "Distributing"
            );

        public Lc PremiumState_Completed = new(
            tr: "Tamamlandı",
            en: "Completed"
            );
        #endregion

        #region ErrorMessages
        public L LoginFailure = new(
            tr: "Oturum açma başarısız",
            en: "Login failure"
        );

        public L DuplicateKeyError = new(
            tr: "Index çakışması hatası",
            en: "Duplicate key error"
        );

        public L CycleName = new (
            tr: "Dönem adı",
            en: "Cycle name"
            );

        public L StartDate = new(
            tr: "Başlangıç tarihi",
            en: "Start date"
            );

        public L EndDate = new(
            tr: "Bitiş tarihi",
            en: "End date"
            );

        /// <summary>
        /// <c>{field}</c> giriniz
        /// </summary>
        public L FieldRequired = new(
            tr: "{field} giriniz",
            en: "{field} required"
            );

        /// <summary>
        /// <c>{field}</c> geçersiz
        /// </summary>
        public L FieldInvalid = new(
            tr: "{field} geçersiz.",
            en: "Invalid {field}."
        );

        /// <summary>
        /// <c>{field}</c> en fazla <c>{maxLength}</c> uzunluğunda olabilir.
        /// </summary>
        public L FieldMaxLength = new(
            tr: "{field} en fazla {maxLength} uzunluğunda olabilir.",
            en: "{field} password."
        );

        public L PasswordsDoesntMatch = new (
            tr: "Parolalar eşleşmiyor.",
            en: "Passwords does not match."
            );

        /// <summary>
        /// Bu e-posta adresi ile kayıtlı bir kullanıcı zaten mevcut.
        /// </summary>
        public L EmailAlreadyExist = new(
            tr: "Bu e-posta adresi ile kayıtlı bir kullanıcı zaten mevcut.",
            en: "This e-mail address already exist."
        );

        public L YourPasswordMustContainAtLeastOneLetterAndOneNumber = new(
            tr: "Parolanız en az bir harf ve bir rakam içermelidir.",
            en: "Your password must contain at least one letter and one number."
        );

        public L InvalidToken = new(
            tr: "Geçersiz oturum anahtarı.",
            en: "Invalid token."
        );

        public L TokenExpired = new(
            tr: "Oturum anahtarı süresi doldu.",
            en: "Token expired."
        );

        public L UserNotAuthorizedOnThisTenant = new(
            tr: "Kullanıcı bu hesapta yetkili değil.",
            en: "The user not authorized on this tenant."
        );

        public L ThereAreEmployeesForThisPosition = new(
            tr: "Bu pozisyona bağlı çalışanlar var.",
            en: "There are employees for this position."
        );

        public L YouCannotModifyADeletedRecord = new(
            tr: "Silinmiş bir kayıt üzerinde değişiklik yapamazsınız.",
            en: "You cannot modify a deleted record."
        );

        /// <summary>
        /// Yönetici çalışanın altında çalışan biri olamaz
        /// </summary>
        public L TheManagerCannotBeSubordinateOfTheEmployee = new(
            tr: "Yönetici çalışanın altında çalışan biri olamaz.",
            en: "The manager cannot be a subordinate of the employee."
        );

        /// <summary>
        /// Üst grup alt gruplardan biri olamaz.
        /// </summary>
        public L TheMainGroupCannotBeOneOfTheSubgroups = new (
            tr: "Üst grup alt gruplardan biri olamaz.",
            en: "The main group cannot be one of the subgroups."
            );

        /// <summary>
        /// Bu doğrulama bağlantısı artık geçerli değil
        /// </summary>
        public L ThisVerificationLinkNoLongerValid = new(
            tr: "Bu doğrulama bağlantısı artık geçerli değil",
            en: "This verification link is no longer valid"
        );

        /// <summary>
        /// Varsayılan dil, erişilebilir dillerden biri olmalı
        /// </summary>
        public L DefaultLanguageMustBeFromAvailableLanguages = new(
            tr: "Varsayılan dil, erişilebilir dillerden biri olmalı.",
            en: "Default language must be from available languages."
        );

        /// <summary>
        /// Prim dönemi zaten <c>{state}</c> modunda
        /// </summary>
        public L PremiumCycleAlreadyInXMode = new(
            tr: "Prim dönemi zaten {state} modunda.",
            en: "Premium cycle is already in {state} mode."
            );

        /// <summary>
        /// {x}, {y}'den önce olamaz.
        /// </summary>
        public L XCannotBeEarlierThanY = new(
            tr: "{x}, {y}'den önce olamaz.",
            en: "{x} cannot be earlier than {y}."
            );

        #endregion
    }
}
