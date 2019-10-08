import { TestBed, inject } from '@angular/core/testing';
import { reducers, metaReducers, State } from '../../core/reducers';
import { AccessTokenInterceptorService } from './access-token-interceptor.service';
import { StoreModule, Store } from '@ngrx/store';

describe('AccessTokenInterceptorService', () => {
  let service: AccessTokenInterceptorService;
  
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [StoreModule.forRoot(reducers)],
      providers: [AccessTokenInterceptorService]
    });
    service = TestBed.get(AccessTokenInterceptorService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should be true', () => {
    expect(false).toBeTruthy();
  })
});
