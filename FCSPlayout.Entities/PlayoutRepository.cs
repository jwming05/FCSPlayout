using FCSPlayout.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Diagnostics;

namespace FCSPlayout.Entities
{
    public class PlayoutRepository
    {
        public static UserEntity GetUser(string name, string password)
        {
            using (var context = new PlayoutDbContext())
            {
                return context.Users.Include(u=>u.Roles).SingleOrDefault(u => u.Name == name && u.Password == password && !u.Locked);
            }
        }

        public static void AddAction(UserAction action)
        {
            //action.CreationTime = DateTime.Now;

            using (var context = new PlayoutDbContext())
            {
                context.UserActions.Add(action);
                context.SaveChanges();
            }
        }

        public static void Register(string machineName, string applicationName)
        {
            using (var context = new PlayoutDbContext())
            {
                bool changed = false;
                var machines = context.Machines.ToList();
                var machine = machines.SingleOrDefault(m => m.Name == machineName);
                MachineInfo newMachine = null;
                if (machine == null)
                {
                    newMachine = new MachineInfo { Name = machineName };
                    context.Machines.Add(newMachine);

                    foreach (var app in context.Applications)
                    {
                        context.SettingScopes.Add(new SettingScope { ApplicationName = app.Name, MachineName = machineName });
                    }
                    changed = true;
                }

                var application = context.Applications.SingleOrDefault(a => a.Name == applicationName);
                if (application == null)
                {
                    context.Applications.Add(new ApplicationInfo { Name = applicationName });
                    bool found = false;
                    foreach (var mac in context.Machines.Local)
                    {
                        if (mac == newMachine)
                        {
                            found = true;
                        }
                        context.SettingScopes.Add(new SettingScope { ApplicationName = applicationName, MachineName = mac.Name });
                    }

                    if (newMachine != null && !found)
                    {
                        Debug.WriteLine("未发现新的MachineName");

                        context.SettingScopes.Add(new SettingScope { ApplicationName = applicationName, MachineName = machineName });
                    }
                    else
                    {
                        Debug.WriteLine("已发现新的MachineName");
                    }
                    changed = true;
                }

                if (changed)
                {
                    context.SaveChanges();
                }
            }
        }

        public static IEnumerable<BMDSwitcherInputInfo> GetSwitcherInputInfos(Guid switcherId, bool includeChannel = false)
        {
            using (var context = new PlayoutDbContext())
            {
                if (includeChannel)
                {
                    return context.BMDSwitcherInputs.Include("Channel").Where(i => i.SwitcherId == switcherId).ToArray();
                }
                else
                {
                    return context.BMDSwitcherInputs.Where(i => i.SwitcherId == switcherId).ToArray();
                }
            }
        }

        public static void Save(IEnumerable<BMDSwitcherInfo> newItems, List<BMDSwitcherInfo> updatedItems, List<BMDSwitcherInfo> removeItems,
            Func<BMDSwitcherInfo, IEnumerable<BMDSwitcherInputInfo>> factory)
        {
            using (var context = new PlayoutDbContext())
            {
                var allItems = context.BMDSwitchers.ToArray();

                foreach (var item in newItems)
                {
                    context.BMDSwitchers.Add(item);

                    foreach (var inputItem in factory(item))
                    {
                        item.InputInfos.Add(inputItem);
                    }
                }


                foreach (var item in updatedItems)
                {
                    var updateItem = allItems.SingleOrDefault(i => i.Id == item.Id);
                    if (updateItem != null)
                    {
                        updateItem.Name = item.Name;
                        updateItem.Address = item.Address;
                    }
                }

                foreach (var item in removeItems)
                {
                    var removeItem = allItems.SingleOrDefault(i => i.Id == item.Id);
                    if (removeItem != null)
                    {
                        context.BMDSwitchers.Remove(removeItem);
                    }
                }
                context.SaveChanges();
            }
        }

        public static void Save(IEnumerable<ChannelInfo> newItems, List<ChannelInfo> updatedItems, List<ChannelInfo> removeItems)
        {
            using (var context = new PlayoutDbContext())
            {
                //var allItems = context.ChannelInfos.ToArray();

                foreach (var item in newItems)
                {
                    context.ChannelInfos.Add(item);
                }

                foreach (var item in updatedItems)
                {
                    context.Entry(item).State = EntityState.Modified;
                }

                foreach (var item in removeItems)
                {
                    context.Entry(item).State = EntityState.Deleted;
                }

                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex)
                {

                }
            }
        }

        public static ChannelInfo GetAutoPaddingChannel()
        {
            var temp = PlayoutRepository.GetSettings("AutoPaddingChannel", null).FirstOrDefault();
            if (temp != null)
            {
                return GetChannelInfo(Guid.Parse(temp.Value));
            }

            return null;
        }

        public static IEnumerable<BMDSwitcherInfo> GetBMDSwitcherInfos()
        {
            using (var context = new PlayoutDbContext())
            {
                return context.BMDSwitchers.ToArray();
            }
        }

        public static ChannelInfo GetChannelInfo(Guid id)
        {
            using (var context = new PlayoutDbContext())
            {
                return context.ChannelInfos.SingleOrDefault(i => i.Id == id);
            }
        }

        public static void AddMediaFile(MediaFileEntity entity, string applicationName, Guid userId, string userName)
        {
            using (var context = new PlayoutDbContext())
            {
                entity.CreatorId = userId;
                context.MediaFiles.Add(entity);
                var action = new UserAction();

                action.ApplicationName = applicationName;

                action.Category = UserActionCategory.Add;
                //action.Data=
                action.Description = string.Format("标题：{0}，原始文件名：{1}，时长：{2}。",
                        entity.Title, entity.OriginalFileName, TimeSpan.FromSeconds(entity.Duration));

                action.Name = "导入素材";
                //action.Tag = "";
                action.UserId = userId;
                action.UserName = userName;
                context.UserActions.Add(action);

                context.SaveChanges();
            }
        }

        public static void SavePlaybill(PlaybillEntity billEntity, IList<PlayItemEntity> playItemEntities)
        {
            using (var context = new PlayoutDbContext())
            {
                if (billEntity.PlaybillItems != null)
                {
                    billEntity.PlaybillItems.Clear();
                }
                else
                {
                    billEntity.PlaybillItems = new List<PlaybillItemEntity>();
                }

                PlaybillEntity entity=billEntity;
                if (entity.Id != Guid.Empty)
                {
                    context.Database.ExecuteSqlCommand("delete from PlaybillItems where PlaybillId=@p0", billEntity.Id);

                    entity = context.Playbills.SingleOrDefault(i => i.Id == billEntity.Id);
                    if (entity == null) return;
                    entity.PlaybillItems = new List<PlaybillItemEntity>();
                    entity.StartTime = billEntity.StartTime;
                    entity.Duration = billEntity.Duration;
                }
                else
                {
                    context.Playbills.Add(entity);
                }

                //List<PlaybillItemEntity> billItemEntities = new List<PlaybillItemEntity>();
                
                for (int i = 0; i < playItemEntities.Count; i++)
                {
                    var playItemEntity = playItemEntities[i];
                    if (!entity.PlaybillItems.Contains(playItemEntity.PlaybillItem))
                    {
                        playItemEntity.PlaybillItem.Playbill = entity;
                        entity.PlaybillItems.Add(playItemEntity.PlaybillItem);
                        //context.PlaybillItems.Add(playItemEntity.PlaybillItem);
                    }
                    context.PlayItems.Add(playItemEntity);
                }

                context.SaveChanges();
                if (billEntity.Id == Guid.Empty)
                {
                    billEntity.Id = entity.Id;
                }
            }
        }

        public static void DeleteMediaFile(MediaFileEntity entity, string applicationName, Guid userId, string userName)
        {
            using (var context = new PlayoutDbContext())
            {
                var item = context.MediaFiles.SingleOrDefault(i => i.Id == entity.Id);
                if (item != null)
                {
                    item.Deleted = true;
                    item.DeleteTime = DateTime.Now;
                    
                    var action = new UserAction();

                    action.ApplicationName = applicationName;

                    action.Category = UserActionCategory.Remove;
                    //action.Data=
                    action.Description = string.Format("素材ID:{3}, 标题：{0}，原始文件名：{1}，时长：{2}。",
                            entity.Title, entity.OriginalFileName, TimeSpan.FromSeconds(entity.Duration), entity.Id);

                    action.Name = "删除素材";
                    //action.Tag = "";
                    action.UserId = userId;
                    action.UserName = userName;

                    context.UserActions.Add(action);
                    context.SaveChanges();
                }
                
            }
        }

        public static PagingItems<MediaFileEntity> GetMediaFiles(MediaItemSearchOptions searchOptions, PagingInfo pagingInfo)
        {
            using (var context = new PlayoutDbContext())
            {
                int rowCount = CountMediaFiles(context,searchOptions);

                var queryable = context.MediaFiles.Include(i=>i.Metadata).Where(i => !i.Deleted);

                queryable = BuildQueryable(queryable, searchOptions);

                var items = queryable
                    .OrderByDescending(i => i.CreationTime)
                    .OrderBy(i => i.Id)
                    .Skip(pagingInfo.RowIndex)
                    .Take(pagingInfo.RowsPerPage)
                    .ToArray();

                return new PagingItems<MediaFileEntity>(pagingInfo, rowCount) { Items = items };
            }
        }

        private static IQueryable<MediaFileEntity> BuildQueryable(IQueryable<MediaFileEntity> queryable, MediaItemSearchOptions searchOptions)
        {
            if (!string.IsNullOrWhiteSpace(searchOptions.Title))
            {
                queryable = queryable.Where(i => i.Title.Contains(searchOptions.Title));
            }

            if (searchOptions.MediaFileCategoryId != null)
            {
                queryable = queryable.Where(i => i.MediaFileCategoryId == searchOptions.MediaFileCategoryId);
            }

            if (searchOptions.MediaFileChannelId != null)
            {
                queryable = queryable.Where(i => i.MediaFileChannelId == searchOptions.MediaFileChannelId);
            }

            if (searchOptions.MinCreationTime != null)
            {
                queryable = queryable.Where(i => i.CreationTime >= searchOptions.MinCreationTime.Value);
            }

            if (searchOptions.MaxCreationTime != null)
            {
                queryable = queryable.Where(i => i.CreationTime <= searchOptions.MaxCreationTime.Value);
            }
            return queryable;
        }

        private static int CountMediaFiles(PlayoutDbContext context, MediaItemSearchOptions searchOptions)
        {
            var queryable = context.MediaFiles.Where(i => !i.Deleted);
            queryable = BuildQueryable(queryable, searchOptions);
            return queryable.Count();
        }

        public static IDictionary<Guid, string> MediaFileCategories
        {
            get
            {
                if (_mediaFileCategories == null)
                {
                    _mediaFileCategories = GetMediaFileCategories();
                    _mediaFileCategories.Add(Guid.Empty, "无");
                }
                return _mediaFileCategories;
            }
        }

        public static IDictionary<Guid, string> MediaFileChannels
        {
            get
            {
                if (_mediaFileChannels == null)
                {
                    _mediaFileChannels = GetMediaFileChannels();
                    _mediaFileChannels.Add(Guid.Empty, "无");
                }
                return _mediaFileChannels;
            }
        }

        private static Dictionary<Guid, string> GetMediaFileCategories()
        {
            Dictionary<Guid, string> result = new Dictionary<Guid, string>();
            using (var context = new PlayoutDbContext())
            {
                var categories = context.MediaFileCategories.ToArray();
                foreach (var c in categories)
                {
                    result.Add(c.Id, c.Name);
                }
            }
            return result;
        }

        private static Dictionary<Guid, string> GetMediaFileChannels()
        {
            Dictionary<Guid, string> result = new Dictionary<Guid, string>();
            using (var context = new PlayoutDbContext())
            {
                var channels = context.MediaFileChannels.ToArray();
                foreach (var c in channels)
                {
                    result.Add(c.Id, c.Name);
                }
            }
            return result;
        }

        private static Dictionary<Guid, string> _mediaFileCategories;
        private static Dictionary<Guid, string> _mediaFileChannels;
        private static MPlaylistSettings _mplaylistSettings;
        public static MPlaylistSettings GetMPlaylistSettings()
        {
            if (_mplaylistSettings == null)
            {
                _mplaylistSettings = new MPlaylistSettings();

                var vf = GetSettings("VideoFormatName", null).FirstOrDefault();
                if (vf != null)
                {
                    _mplaylistSettings.VideoFormat = vf.Value; // Properties.Settings.Default.VideoFormatName;
                }

                var af = GetSettings("AudioFormatName", null).FirstOrDefault();
                if (af != null)
                {
                    _mplaylistSettings.AudioFormat = af.Value; // Properties.Settings.Default.AudioFormatName;
                }
            }

            return _mplaylistSettings;
        }

        public static IEnumerable<SettingInfo> GetSettings(string name, string groupName)
        {
            groupName = groupName ?? PlayoutDbContext.DefaultName;

            using (var context = new PlayoutDbContext())
            {
                return context.Settings.Include("Scope").Where(i => i.Name == name && i.GroupName == groupName).ToArray();
            }
        }

        public static IEnumerable<ChannelInfo> GetChannelInfos(bool excludeSpecial = false)
        {
            using (var context = new PlayoutDbContext())
            {
                //var result= ;
                if (excludeSpecial)
                {
                    return context.ChannelInfos.Where(i => !i.Special).ToArray();
                }
                else
                {
                    return context.ChannelInfos.ToArray();
                }

            }
        }

        public static PlaybillEntity GetPlaybill(Guid id)
        {
            using (var context = new PlayoutDbContext())
            {
                var billEntity = context.Playbills
                    .Include(i => i.PlaybillItems.Select(p => p.PlayItems))
                    .Include(i => i.PlaybillItems.Select(p => p.MediaSource))
                    .SingleOrDefault(i => i.Id == id);

                if (billEntity != null)
                {
                    billEntity.PlayItems = billEntity.PlaybillItems.SelectMany(i => i.PlayItems).OrderBy(i => i.StartTime).ToList();
                    //billEntity.PlayItems.Sort(new Comparison<PlayItemEntity>((x,y)=>(int)x.GetStartTime().Subtract(y.GetStartTime()).TotalMilliseconds));
                }

                return billEntity;
            }
        }

        public static void SaveSetting(string machineName, string applicationName, string groupName, string name, string value, string tag)
        {
            machineName = machineName ?? PlayoutDbContext.DefaultName;
            applicationName = applicationName ?? PlayoutDbContext.DefaultName;

            using (var context = new PlayoutDbContext())
            {
                var scope = context.SettingScopes.Single(i => i.MachineName == machineName && i.ApplicationName == applicationName);
                groupName = groupName ?? PlayoutDbContext.DefaultName;

                var dbEntity = context.Settings.SingleOrDefault(s => s.ScopeId == scope.Id && s.Name == name && s.GroupName == groupName);
                if (dbEntity != null)
                {
                    dbEntity.Value = value;
                    dbEntity.Tag = tag;
                }
                else
                {
                    var setting = new SettingInfo
                    {
                        ScopeId = scope.Id,
                        GroupName = groupName,
                        Name = name,
                        Value = value,
                        Tag = tag
                    };

                    context.Settings.Add(setting);
                }

                context.SaveChanges();
            }
        }

        public static void Save(Guid switcherId, IEnumerable<BMDSwitcherInputInfo> switcherInputInfos)
        {
            using (var context = new PlayoutDbContext())
            {
                var dbEntities = context.BMDSwitchers.Include("InputInfos")
                    .Single(i => i.Id == switcherId).InputInfos; // context.BMDSwitcherInputs.Where(i=>i.SwitcherId==switcherId).ToArray();
                foreach (var info in switcherInputInfos)
                {
                    var dbEntity = dbEntities.SingleOrDefault(i => i.Id == info.Id);
                    if (dbEntity != null && dbEntity.ChannelId != info.ChannelId)
                    {
                        dbEntity.ChannelId = info.ChannelId;
                    }
                }

                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex)
                {

                }
            }
        }

        public static void SaveMPlaylistSettings(MPlaylistSettings settings)
        {
            SaveSetting(null, null, null, "VideoFormatName", settings.VideoFormat, null);
            SaveSetting(null, null, null, "AudioFormatName", settings.AudioFormat, null);

            _mplaylistSettings = null;
        }

        public static Guid AddPlayRecord(PlayRecord playRecord)
        {
            using (var context = new PlayoutDbContext())
            {
                context.PlayRecords.Add(playRecord);
                context.SaveChanges();
                return playRecord.Id;
            }
        }
    }
}