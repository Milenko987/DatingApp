using API.DTO;
using API.Entitites;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userrepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {

            _userrepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserToReturnDTO>>> GetUsers()
        {
            var users = await _userrepository.GetUsers();
            var usersToReturnList = _mapper.Map<IEnumerable<UserToReturnDTO>>(users);
            return Ok(usersToReturnList);
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserToReturnDTO>> GetUserById(int id)
        {
            var user = await _userrepository.GetUserById(id);
            var usersToReturn = _mapper.Map<UserToReturnDTO>(user);

            return Ok(usersToReturn);
        }


        [Authorize]
        [HttpGet("{username}")]
        public async Task<ActionResult<UserToReturnDTO>> GetUserByUsername(string username)
        {
            var user = await _userrepository.GetUserByUserName(username);
            var usersToReturn = _mapper.Map<UserToReturnDTO>(user);

            return Ok(usersToReturn);


        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult> UpdateUser(UserUpdateDTO userUpdateDTO)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userrepository.GetUserByUserName(username);
            if (user == null)
            {
                return NotFound();
            }

            _mapper.Map(userUpdateDTO, user);
            if (await _userrepository.SaveAllAsync())
            {
                return NoContent();
            }
            return BadRequest("Failed to update user");
        }

        [Authorize]
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userrepository.GetUserByUserName(username);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _photoService.UploadPhoto(file);


            var photo = new Photo
            {
                Url = result.url,
                PublicId = result.filePath
            };
            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }
            user.Photos.Add(photo);
            if (await _userrepository.SaveAllAsync())
            {
                return _mapper.Map<PhotoDto>(photo);
            }
            return BadRequest("Image upload failed");
        }

        [Authorize]
        [HttpPut("set-main-photo")]
        public async Task<ActionResult> SetMainPhoto([FromBody] int photoId)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userrepository.GetUserByUserName(username);
            if (user == null)
            {
                return NotFound();
            }
            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);
            if (photo == null)
            {
                return NotFound();
            }
            var currentMainPhoto = user.Photos.FirstOrDefault(a => a.IsMain);
            if (!photo.IsMain)
            {
                photo.IsMain = true;
                currentMainPhoto.IsMain = false;
            }
            if (await _userrepository.SaveAllAsync())
            {
                return NoContent();
            }
            else
            {
                return BadRequest("Could not set main photo");
            }
        }

        [Authorize]
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userrepository.GetUserByUserName(username);
            if (user == null)
            {
                return NotFound();
            }
            var photoToDelete = user.Photos.Find(p => p.Id == photoId);
            if (photoToDelete == null)
            {
                return NotFound();
            }
            if (photoToDelete.IsMain)
            {
                return BadRequest("You can not delete main photo");
            }
            var result = _photoService.DeletePhoto(photoToDelete.Id.ToString());
            user.Photos.Remove(photoToDelete);
            await _userrepository.SaveAllAsync();
            return Ok();
        }



    }
}
