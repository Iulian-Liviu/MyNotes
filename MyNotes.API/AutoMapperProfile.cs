using AutoMapper;
using MyNotes.API.Models;
using MyNotes.API.Models.DtoModels;

namespace MyNotes.API
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            this.CreateMap<NoteUpload, Note>();
            this.CreateMap<Note, NoteResponse>();
        }
    }
}
