﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using Cvent.SchemaToPoco.Core.Util;
using Newtonsoft.Json.Schema;

namespace Cvent.SchemaToPoco.Core.Types
{
    /// <summary>
    ///     Wrapper for an automatic property.
    /// </summary>
    internal class PropertyWrapper : BaseWrapper<CodeMemberProperty>
    {
        public PropertyWrapper(CodeMemberProperty p) : base(p)
        {
        }

        /// <summary>
        ///     Add all comments and attributes.
        /// </summary>
        /// <param name="schema">The JsonSchema.</param>
        public void Populate(JsonSchema schema)
        {
            // Add description
            if (schema.Description != null)
            {
                AddComment(schema.Description);
            }

            // Add required attribute
            if (schema.Required != null && schema.Required.Value)
            {
                AddAttribute("Required");
            }

            // Number only flags
            if (JsonSchemaUtils.IsNumber(schema))
            {
                if (schema.Minimum != null)
                {
                    if (schema.ExclusiveMinimum != null && schema.ExclusiveMinimum.Value)
                    {
                        AddAttribute("MinValue", (int) schema.Minimum.Value + 1);
                    }
                    else
                    {
                        AddAttribute("MinValue", (int) schema.Minimum.Value);
                    }
                }
                if (schema.Maximum != null)
                {
                    if (schema.ExclusiveMaximum != null && schema.ExclusiveMaximum.Value)
                    {
                        AddAttribute("MaxValue", (int) schema.Maximum.Value - 1);
                    }
                    else
                    {
                        AddAttribute("MaxValue", (int) schema.Maximum.Value);
                    }
                }
            }

            // String only flags
            if (JsonSchemaUtils.IsString(schema))
            {
                var args = new List<CodeAttributeArgument>();
                bool flag = false;

                if (schema.MaximumLength != null)
                {
                    args.Add(new CodeAttributeArgument(new CodePrimitiveExpression(schema.MaximumLength.Value)));
                    flag = true;
                }

                if (schema.MinimumLength != null)
                {
                    args.Add(new CodeAttributeArgument("MinimumLength",
                        new CodePrimitiveExpression(schema.MinimumLength.Value)));
                    flag = true;
                }

                if (flag)
                {
                    AddAttribute("StringLength", args.ToArray());
                }

                if (!String.IsNullOrEmpty(schema.Pattern))
                {
                    AddAttribute("RegularExpression",
                        new CodeAttributeArgument(new CodeSnippetExpression(string.Format(@"@""{0}""",
                            StringUtils.SanitizeRegex(schema.Pattern, true)))));
                }
            }

            // Array only flags
            if (JsonSchemaUtils.IsArray(schema))
            {
                if (schema.MinimumItems != null)
                {
                    AddAttribute("MinLength", schema.MinimumItems.Value);
                }
                if (schema.MaximumItems != null)
                {
                    AddAttribute("MaxLength", schema.MaximumItems.Value);
                }
            }
        }
    }
}
