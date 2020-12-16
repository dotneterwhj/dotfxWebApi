using AutoMapper;
using DotneterWhj.DataTransferObject;
using DotneterWhj.EntityModel;
using DotneterWhj.IServices;
using DotneterWhj.Repository;
using DotneterWhj.Repository.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotneterWhj.Services
{
    public class AppInfoService : BaseService<AppInfoDto, AppInfo>, IAppInfoService
    {
        public AppInfoService(IRepository<AppInfo> repository, IUnitOfWork unitOfWork, IMapper mapper)
            : base(repository, unitOfWork, mapper)
        {

        }
    }
}
