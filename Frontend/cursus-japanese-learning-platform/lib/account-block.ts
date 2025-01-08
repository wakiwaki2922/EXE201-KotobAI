
// export const isInstructorBlocked = () => {
//   if (typeof window === "undefined") {
//     return false;
//   }
//   const userDataString = localStorage.getItem("userData");
//   if (!userDataString) {
//     return false;
//   }

//   const userData = JSON.parse(userDataString);

//   return userData.status === "BLOCK_ROLE_INSTRUCTOR";
// };

export const isStudentBlocked = () => {
  if (typeof window === "undefined") {
    return false;
  }
  const userDataString = localStorage.getItem("userData");
  if (!userDataString) {
    return false;
  }

  const userData = JSON.parse(userDataString);

  return userData.isDelete === "BLOCK_ROLE_STUDENT";
};

export const isAccountVerified = () => {
  if (typeof window === "undefined") {
    return false;
  }
  const userDataString = localStorage.getItem("userData");
  if (!userDataString) {
    return false;
  }

  const userData = JSON.parse(userDataString);

  return userData.isActive === true;
};