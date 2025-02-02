// --------------------------------------------------------------------------
// <copyright file="VariableHelpers.cs" company="-">
// Copyright (c) 2008-2017 Eser Ozvataf (eser@ozvataf.com). All rights reserved.
// Web: http://eser.ozvataf.com/ GitHub: http://github.com/eserozvataf
// </copyright>
// <author>Eser Ozvataf (eser@ozvataf.com)</author>
// --------------------------------------------------------------------------

//// This program is free software: you can redistribute it and/or modify
//// it under the terms of the GNU General Public License as published by
//// the Free Software Foundation, either version 3 of the License, or
//// (at your option) any later version.
//// 
//// This program is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//// GNU General Public License for more details.
////
//// You should have received a copy of the GNU General Public License
//// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Tassle.Helpers {
    /// <summary>
    /// VariableUtils class.
    /// </summary>
    public static class VariableHelpers {
        // methods

        /// <summary>
        /// Checks the and dispose.
        /// </summary>
        /// <param name="variable">The variable</param>
        public static void CheckAndDispose<T>(ref T variable) where T : class, IDisposable {
            if (variable != null) {
                variable.Dispose();
                variable = null;
            }
        }

        /// <summary>
        /// Gets the member type.
        /// </summary>
        /// <param name="member">The member</param>
        /// <returns>Member type</returns>
        public static Type GetMemberType(MemberInfo member) {
            if (member is FieldInfo fieldInfoMember) {
                return fieldInfoMember.FieldType;
            }

            if (member is PropertyInfo propertyInfoMember) {
                return propertyInfoMember.PropertyType;
            }

            return null;
        }

        /// <summary>
        /// Reads the member value.
        /// </summary>
        /// <param name="member">The member</param>
        /// <param name="instance">The instance</param>
        /// <param name="enumAsString">Whether serialize enum as string or not</param>
        /// <returns>Value of the field/property instance</returns>
        public static object ReadMemberValue(MemberInfo member, object instance, bool enumAsString = false) {
            object value = null;

            if (member is FieldInfo fieldInfoMember) {
                value = fieldInfoMember.GetValue(instance);

                if (fieldInfoMember.FieldType.GetTypeInfo().IsEnum) {
                    if (enumAsString) {
                        value = value.ToString();
                    }
                    else {
                        value = Convert.ChangeType(value, Enum.GetUnderlyingType(fieldInfoMember.FieldType));
                    }
                }
            }
            else if (member is PropertyInfo propertyInfoMember) {
                value = propertyInfoMember.GetValue(instance, null);

                if (propertyInfoMember.PropertyType.GetTypeInfo().IsEnum) {
                    if (enumAsString) {
                        value = value.ToString();
                    }
                    else {
                        value = Convert.ChangeType(value, Enum.GetUnderlyingType(propertyInfoMember.PropertyType));
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// Writes the member value.
        /// </summary>
        /// <param name="member">The member</param>
        /// <param name="instance">The instance</param>
        /// <param name="value">The value</param>
        /// <param name="enumAsString">Whether serialize enum as string or not</param>
        public static void WriteMemberValue(MemberInfo member, object instance, object value, bool enumAsString = false) {
            FieldInfo fieldInfo = null;
            PropertyInfo propertyInfo = null;
            Type type;

            if (member is FieldInfo fieldInfoMember) {
                type = Nullable.GetUnderlyingType(fieldInfoMember.FieldType) ?? fieldInfoMember.FieldType;
            }
            else if (member is PropertyInfo propertyInfoMember) {
                type = Nullable.GetUnderlyingType(propertyInfoMember.PropertyType) ?? propertyInfoMember.PropertyType;
            }
            else {
                return;
            }

            if (value != null) {
                if (type.GetTypeInfo().IsEnum && enumAsString) {
                    value = Enum.Parse(type, value.ToString());
                }
                else if (type != value.GetType()) {
                    value = Convert.ChangeType(value, type);
                }
            }

            if (member is FieldInfo) {
                fieldInfo.SetValue(instance, value);
            }
            else if (member is PropertyInfo) {
                propertyInfo.SetValue(instance, value);
            }
        }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <returns>Size of the type</returns>
        public static int GetSize<T>() {
            var type = typeof(T);

            if (type.GetTypeInfo().IsValueType) {
                // value type
                return Marshal.SizeOf<T>();
            }

            // a reference
            return IntPtr.Size;
        }
    }
}
