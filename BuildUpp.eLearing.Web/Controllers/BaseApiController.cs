using BuildUpp.eLearing.Web.Models;
using BuildUpp.eLearning.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace BuildUpp.eLearing.Web.Controllers
{
    public class BaseApiController : ApiController
    {
        private ILearningRepository _repository;
        private ModelFactory _modelFactory;

        public BaseApiController(ILearningRepository repository)
        {
            _repository = repository;
        }

        protected ILearningRepository Repository
        {
            get
            {
                return _repository;
            }
        }

        protected ModelFactory TheModelFactory
        {
            get
            {
                if (_modelFactory == null)
                {
                    _modelFactory = new ModelFactory(Request, Repository);
                }
                return _modelFactory;
            }
        }
    }
}