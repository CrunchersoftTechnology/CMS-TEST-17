namespace CMS.Domain.Storage.Services
{
   public interface IApiService
    {
        string  GetNews();
        //void GetNewsPDFById(int id);
        string GetNotes();
        string GetReferencebooks();
        string GetAssignments();
        string GetTextbooks();
        string GetInbuiltquestionbank();
        string GetPracticepapers();
        string GetQuestionpapers();


        string GetTest();
        string GetAdmission(int? Id);
        string GetTestByUploadId(int? Id);

        string GetNotesByUploadId(int? Id);
        string GetAssignmentByUploadId(int? Id);
        string GetTextbooksByUploadId(int? Id);
        string GetInbuiltquestionbankByUploadId(int? Id);
        string GetPracticepapersByUploadId(int? Id);
        string GetQuestionpapersByUploadId(int? Id);
        string GetReferencebooksByUploadId(int? Id);


        string UpdateAdmission(string Email);
        string GetPendingAdmission();
        string GetPendingAdmissionList();


        string GetClasses();
        string GetBoards();
        string GetSubjects(int classId);
        string GetTeachers();
        string GetSchool();
        string GetBranch();
        string GetBatch();
        string GetStudentFeedback();
        string GetClassesByMultipleBranch(string selectedBranch);
        //string PDFCategoryApi(string category);
    }
}
