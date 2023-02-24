using AutoMapper;
using MyNotes.API.Models;
using MyNotes.API.Models.DtoModels;

namespace MyNotes.API;

public class AutoMapperProfile : Profile
{
    /* This AutoMapperProfile class creates mappings between different types of objects.
     It maps the NoteUpload and Note objects, 
     as well as the UserUpload and User objects, to their respective responses.
     */
    public AutoMapperProfile()
    {
        CreateMap<NoteUpload, Note>();
        CreateMap<Note, NoteResponse>();


        CreateMap<UserUpload, User>();
        CreateMap<User, UserResponse>();
    }
}