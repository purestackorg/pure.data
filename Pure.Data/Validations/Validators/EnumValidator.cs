﻿
namespace Pure.Data.Validations.Validators
{
	using System;
	using System.Reflection;
	using Pure.Data.Validations.Internal;
	using Resources;
    using Pure.Data.i18n;

	public class EnumValidator : PropertyValidator
	{
		private readonly Type enumType;

		public EnumValidator(Type enumType) : base(() => Messages.enum_error) {
			this.enumType = enumType;
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			if (context.PropertyValue == null) return true;

			var underlyingEnumType = Nullable.GetUnderlyingType(enumType) ?? enumType;

			if (!underlyingEnumType.GetTypeInfo().IsEnum) return false;

			if (underlyingEnumType.GetTypeInfo().GetCustomAttribute<FlagsAttribute>() != null)
			{
				return IsFlagsEnumDefined(underlyingEnumType, context.PropertyValue);
			}

			return Enum.IsDefined(underlyingEnumType, context.PropertyValue);
		}

		private static bool IsFlagsEnumDefined(Type enumType, object value) {
			var typeName = Enum.GetUnderlyingType(enumType).Name;

			switch (typeName)
			{
				case "Byte":
					{
						var typedValue = (byte)value;
						return EvaluateFlagEnumValues(typedValue, enumType);
					}

				case "Int16":
					{
						var typedValue = (short)value;

						if (typedValue < 0)
						{
							return false;
						}

						return EvaluateFlagEnumValues(Convert.ToUInt64(typedValue), enumType);
					}

				case "Int32":
					{
						var typedValue = (int)value;

						if (typedValue < 0)
						{
							return false;
						}

						return EvaluateFlagEnumValues(Convert.ToUInt64(typedValue), enumType);
					}

				case "Int64":
					{
						var typedValue = (long)value;

						if (typedValue < 0)
						{
							return false;
						}

						return EvaluateFlagEnumValues(Convert.ToUInt64(typedValue), enumType);
					}

				case "SByte":
					{
						var typedValue = (sbyte)value;

						if (typedValue < 0)
						{
							return false;
						}

						return EvaluateFlagEnumValues(Convert.ToUInt64(typedValue), enumType);
					}

				case "UInt16":
					{
						var typedValue = (ushort)value;
						return EvaluateFlagEnumValues(typedValue, enumType);
					}

				case "UInt32":
					{
						var typedValue = (uint)value;
						return EvaluateFlagEnumValues(typedValue, enumType);
					}

				case "UInt64":
					{
						var typedValue = (ulong)value;
						return EvaluateFlagEnumValues(typedValue, enumType);
					}

				default:
					var message = string.Format("Unexpected typeName of '{0}' during flags enum evaluation.", typeName);
					throw new ArgumentOutOfRangeException("typeName", message);
			}
		}

		private static bool EvaluateFlagEnumValues(ulong value, Type enumType) {
			ulong mask = 0;

			foreach (var enumValue in Enum.GetValues(enumType)) {
				var enumValueAsUInt64 = Convert.ToUInt64(enumValue);
				mask = mask | enumValueAsUInt64;
			}

			return (mask & value) == value;
		}
	}
}