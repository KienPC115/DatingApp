import { ActivatedRouteSnapshot, DetachedRouteHandle, RouteReuseStrategy } from "@angular/router";

// Lesson 236. Notifying users when they receive a message
// problem là khi user nhấn vào thoogn báo tin nhắn mới nó sẽ tới component thông qua navigateByUrl- Route
/*
    Khi bạn thực hiện navigation đến một route mới, 
    Angular Router sẽ kiểm tra shouldReuseRoute để xác định liệu route cần tái sử dụng hay không. 
    Trong trường hợp của bạn, vì shouldReuseRoute trả về false, Angular sẽ tạo mới một thành phần 
    route mới thay vì tái sử dụng từ cache.
    => vì khi bạn navigate tới route of member-message component nó đã không load - didnot do any inside component
    => apply RouteReuseStrategy và set up các giá trị false -> nó sẽ load lại route và load component
*/
export class CustomRouteReuseStrategy implements RouteReuseStrategy {
    shouldDetach(route: ActivatedRouteSnapshot): boolean {
        return false;
    }

    store(route: ActivatedRouteSnapshot, handle: DetachedRouteHandle | null): void {
    }

    shouldAttach(route: ActivatedRouteSnapshot): boolean {
        return false;
    }

    retrieve(route: ActivatedRouteSnapshot): DetachedRouteHandle | null {
        return null;
    }

    shouldReuseRoute(future: ActivatedRouteSnapshot, curr: ActivatedRouteSnapshot): boolean {
        return false;
    }


}