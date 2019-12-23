using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;

namespace Helpers.Extensions
{
    public static class JsonPatchDocumentExtensions
    {
        public static void ApplyTo<T, U>(this JsonPatchDocument<U> inPatch, T obj, IMapper mapper) 
            where U: class
            where T: class
        {
            var outPatch = mapper.Map<JsonPatchDocument<T>>(inPatch);
            outPatch.ApplyTo(obj);
        }
    }
}
