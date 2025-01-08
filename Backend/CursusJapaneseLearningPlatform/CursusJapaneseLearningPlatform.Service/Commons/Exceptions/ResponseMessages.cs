﻿namespace CursusJapaneseLearningPlatform.Service.Commons.Exceptions;

public static class ResponseMessages
{
    public const string NOT_FOUND = "Không tìm thấy {0}.";
    public const string SUCCESS = "Thành công!";
    public const string FAILED = "Thất bại!";
    public const string EXISTED = "{0} đã tồn tại.";
    public const string DUPLICATE = "{0} bị trùng lặp.";
    //public const string INTERNAL_SERVER_ERROR = "Lỗi máy chủ nội bộ!";
    public const string INTERNAL_SERVER_ERROR = "Đã có lỗi xảy ra";
    public const string INVALID_INPUT = "Dữ liệu đầu vào không hợp lệ!";
    public const string UNAUTHORIZED = "Không có quyền truy cập!";
    public const string BADREQUEST = "Yêu cầu không hợp lệ!";
    public const string ERROR = "Lỗi!";
    public const string CHAT_NOT_FOUND = "Không tìm thấy cuộc trò chuyện.";

    // Success messages
    public const string CREATE_SUCCESS = "Tạo {0} thành công!";
    public const string UPDATE_SUCCESS = "Cập nhật {0} thành công!";
    public const string DELETE_SUCCESS = "Xóa {0} thành công!";
    public const string GET_SUCCESS = "Lấy thông tin {0} thành công!";
    public const string PROCESS_SUCCESS = "Xử lý {0} thành công!";
    public const string UPLOAD_SUCCESS = "Tải lên {0} thành công!";
    public const string SUBMIT_SUCCESS = "Gửi {0} thành công!";
    public const string SAVE_SUCCESS = "Lưu {0} thành công!";
    public const string SEND_SUCCESS = "Gửi mail {0} thành công!";

    // Error messages
    public const string CREATE_FAIL = "Tạo {0} thất bại!";
    public const string SEND_FAIL = "Gửi mail {0} thất bại!";
    public const string UPDATE_FAIL = "Cập nhật {0} thất bại!";
}
