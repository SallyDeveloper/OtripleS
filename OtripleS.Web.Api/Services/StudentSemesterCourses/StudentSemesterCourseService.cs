﻿//---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE AS LONG AS SOFTWARE FUNDS ARE DONATED TO THE POOR
//----------------------------------------------------------------

using System;
using System.Linq;
using OtripleS.Web.Api.Brokers.DateTimes;
using OtripleS.Web.Api.Brokers.Loggings;
using OtripleS.Web.Api.Brokers.Storage;
using OtripleS.Web.Api.Models.StudentSemesterCourses;
using System.Threading.Tasks;

namespace OtripleS.Web.Api.Services.StudentSemesterCourses
{
    public partial class StudentSemesterCourseService : IStudentSemesterCourseService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;

        public StudentSemesterCourseService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public ValueTask<StudentSemesterCourse> CreateStudentSemesterCourseAsync(
            StudentSemesterCourse studentSemesterCourse) =>
            TryCatch(async () =>
            {
                ValidateStudentSemesterCourseOnCreate(studentSemesterCourse);
                return await this.storageBroker.InsertStudentSemesterCourseAsync(studentSemesterCourse);
            });

        public IQueryable<StudentSemesterCourse> RetrieveAllStudentSemesterCourses() =>
            TryCatch(() =>
            {
                IQueryable<StudentSemesterCourse> storageStudentSemesterCourses =
                    this.storageBroker.SelectAllStudentSemesterCourses();

                ValidateStorageStudentSemesterCourses(storageStudentSemesterCourses);
                return storageStudentSemesterCourses;
            });

        public ValueTask<StudentSemesterCourse> ModifyStudentSemesterCourseAsync(StudentSemesterCourse studentSemesterCourse) =>
            TryCatch(async () =>
            {
                ValidateStudentSemesterCourseOnModify(studentSemesterCourse);
                StudentSemesterCourse maybeStudentSemesterCourse =
                    await this.storageBroker
                    .SelectStudentSemesterCourseByIdAsync
                    (studentSemesterCourse.StudentId, studentSemesterCourse.SemesterCourseId);

                return await this.storageBroker.UpdateStudentSemesterCourseAsync(studentSemesterCourse);
            });

        public ValueTask<StudentSemesterCourse>
            DeleteStudentSemesterCourseAsync(Guid semesterCourseId, Guid studentId) =>
            TryCatch(async () =>
            {
                ValidateSemesterCourseId(semesterCourseId);
                ValidateStudentId(studentId);
                StudentSemesterCourse studentSemesterCourse =
                    await this.storageBroker.SelectStudentSemesterCourseByIdAsync(semesterCourseId, studentId);
                ValidateStorageStudentSemesterCourse(studentSemesterCourse, semesterCourseId, studentId);

                return await this.storageBroker.DeleteStudentSemesterCourseAsync(studentSemesterCourse);
            });
    }
}
