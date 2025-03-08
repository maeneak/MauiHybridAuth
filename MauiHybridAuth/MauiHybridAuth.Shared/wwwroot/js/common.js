//function readProfilePicture(input) {
//    if (input.files && input.files[0]) {
//        var reader = new FileReader();
//        reader.onload = function (e) {
//            var base64String = e.target.result.split(',')[1];
//            DotNet.invokeMethodAsync('MauiHybridAuth.Web', 'UpdateProfilePicture', base64String);
//        };
//        reader.readAsDataURL(input.files[0]);
//    }
//}