import { TestBed, inject } from '@angular/core/testing';
import { reducers, metaReducers, State } from '../../core/reducers';
import { StoreModule, Store } from '@ngrx/store';
import { AuthorizationErrorResponseInterceptorService } from './authorization-error-response-interceptor.service';

describe('AuthorizationErrorInterceptorService', () => {
  let service: AuthorizationErrorResponseInterceptorService;
  
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [StoreModule.forRoot(reducers)],
      providers: [AuthorizationErrorResponseInterceptorService]
    });
    service = TestBed.get(AuthorizationErrorResponseInterceptorService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});