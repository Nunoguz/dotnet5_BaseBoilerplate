using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nunoguz_Boilerplate.Controllers.Base.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BaseV1ApiController : BaseApiController
    {
    }
}
